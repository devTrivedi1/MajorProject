using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Dreamteck;

public class GrindController : MonoBehaviour
{
    SplineFollower splineFollower;
    SplineProjector splineProjector;

    [SerializeField] float speedMultiplier;
    [SerializeField] float transitionSpeed;
    float startSpeed;
    [SerializeField] float distanceToSwitchNode;
    [SerializeField] float distanceToGoOnRail;

    [SerializeField] List<Node> AllNodes = new List<Node>();
    [SerializeField] List<SplineComputer> AllSplines = new List<SplineComputer>();
    [SerializeField] Node.Connection currentConnection;
    [SerializeField] Node closestNode;

    public bool isGrinding = false;
    bool isSpeedingUp;
    bool isInputPressed = false;
    [SerializeField] bool isSwitchingTrack = false;

    public static Action<bool> OnRailGrindStateChange;
    public static Action<JumpState> TriggerJumpingOffRails;


    private void OnEnable()
    {
        splineFollower = GetComponent<SplineFollower>();
        splineProjector = GetComponent<SplineProjector>();
        startSpeed = splineFollower.followSpeed;
        Node[] nodeArray = FindObjectsOfType<Node>();
        AllNodes = nodeArray.ToList();
        SplineComputer[] splineArray = FindObjectsOfType<SplineComputer>();
        AllSplines = splineArray.ToList();
        splineFollower.onNode += OnJunction;
        Jump.OnJumpStateChanged += GetOffRailsOnJump;
        Jump.GetExternalMomentum += JumpMomentumAddon;

    }

    private void OnDisable()
    {
        splineFollower.onNode -= OnJunction;
        Jump.OnJumpStateChanged -= GetOffRailsOnJump;
        Jump.GetExternalMomentum -= JumpMomentumAddon;
    }

    private void Update()
    {
        // no spline reference, then no grinding

        if (splineFollower.spline == null) { splineFollower.follow = false; return; }

        transform.GetChild(0).gameObject.transform.rotation = transform.rotation;

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (splineFollower.direction == Spline.Direction.Forward)
            {
                float value = -splineFollower.followSpeed;
                splineFollower.followSpeed = value;
            }
            else
            {
                float value = Math.Abs(splineFollower.followSpeed);
                splineFollower.followSpeed = value;
            }

        }

        GrindRailSprinting();

        // finding the closest node
        AllNodes.ForEach(node =>
        {
            float distance = Vector3.Distance(transform.position, node.transform.position);
            if (distance < distanceToSwitchNode)
            {
                closestNode = node;
                return;
            }

        });

        RailSwitchingByInput();

        JumpOffAtTheEndOfRail();
    }



    private void RailSwitchingByInput()
    {
        if (closestNode != null)
        {
            float distance = Vector3.Distance(transform.position, closestNode.transform.position);
            // this is to choose a track when input is pressed

            if (Input.GetKeyDown(KeyCode.D))
            {

                if (Vector3.Distance(transform.position, closestNode.transform.position) > distanceToSwitchNode) return;

                foreach (var connection in closestNode.GetConnections())
                {
                    if (connection.spline == splineFollower.spline)
                    {
                        currentConnection = connection;
                        break;
                    }
                }

                Node.Connection mostConvenientConnection = WithInputGetMostConvenientConnection(KeyCode.D);
                if (mostConvenientConnection == null) return;

                if (mostConvenientConnection.spline != splineFollower.spline)
                {

                    isInputPressed = true;
                    Invoke(nameof(disableInputEnabled), 0.15f);
                    SwitchSpline(currentConnection, mostConvenientConnection);
                    closestNode = null;
                    return;
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {

                if (Vector3.Distance(transform.position, closestNode.transform.position) > distanceToSwitchNode) return;



                foreach (var connection in closestNode.GetConnections())
                {
                    if (connection.spline == splineFollower.spline)
                    {
                        currentConnection = connection;
                        break;
                    }
                }

                Node.Connection mostConvenientConnection = WithInputGetMostConvenientConnection(KeyCode.A);
                if (mostConvenientConnection == null) return;

                if (mostConvenientConnection.spline != splineFollower.spline)
                {

                    isInputPressed = true;
                    Invoke(nameof(disableInputEnabled), 0.15f);
                    SwitchSpline(currentConnection, mostConvenientConnection);
                    closestNode = null;
                    return;
                }
            }
        }
    }

    private void GrindRailSprinting()
    {
        isSpeedingUp = Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = isSpeedingUp ? startSpeed * speedMultiplier : startSpeed;
        targetSpeed = splineFollower.direction == Spline.Direction.Forward ? targetSpeed : -targetSpeed;
        splineFollower.followSpeed = Mathf.Lerp(splineFollower.followSpeed, targetSpeed, transitionSpeed * Time.deltaTime);
    }

    Node.Connection WithInputGetMostConvenientConnection(KeyCode keyPress)
    {
        if (keyPress == KeyCode.D)
        {
            Node.Connection[] availableConnections = closestNode.GetConnections();
            return availableConnections[0];
        }
        else if (keyPress == KeyCode.A)
        {
            Node.Connection[] availableConnections = closestNode.GetConnections();
            return availableConnections[2];
        }
        return null;
    }

    // this  will choose a default track if the current track is closed and no input is pressed
    private void OnJunction(List<SplineTracer.NodeConnection> passed)
    {
        if (isInputPressed || !isGrinding) return;

        if (closestNode == null) return;

        bool ProceedSwitchingProccess = ShouldSwitchRailMovingForward();
        if (ProceedSwitchingProccess) { return; }
        Node.Connection[] availableConnections = new Node.Connection[closestNode.GetConnections().Length];
        closestNode.GetConnections().CopyTo(availableConnections, 0);

        GetCurrentConnection(availableConnections);

        foreach (var connection in closestNode.GetConnections())
        {

            if (!currentConnection.spline.isClosed && connection.spline != splineFollower.spline)
            {
                if (isSwitchingTrack) { return; }
                isSwitchingTrack = true;

                SwitchSpline(currentConnection, connection);
                closestNode = null;
                return;
            }
        }
    }


    bool ShouldSwitchRailMovingForward()
    {
        RaycastHit[] colliderHits = Physics.SphereCastAll(transform.position, 1f, transform.forward, 50);

        foreach (var hit in colliderHits)
        {
            if (hit.collider.TryGetComponent(out SplineComputer spline))
            {
                if (splineFollower.result.percent < 0.8f) { return true; }

            }
        }
        return false;
    }
    void SwitchSpline(Node.Connection from, Node.Connection to)
    {
        splineFollower.spline = to.spline;
        splineFollower.RebuildImmediate();
        double startpercent = splineFollower.ClipPercent(to.spline.GetPointPercent(to.pointIndex));
        splineFollower.SetPercent(startpercent);
        if (to.spline.isClosed) return;
        if (splineFollower.result.percent < 0.5)
        {
            splineFollower.followSpeed = startSpeed;
        }
        else
        {
            splineFollower.followSpeed = -startSpeed;
        }
        Invoke(nameof(DisableSwitchingTrack), 0.1f);
    }

    public void GoGrindOnThoseRails(SplineComputer splineToGrindOn)
    {
        if (!isGrinding)
        {

            splineFollower.spline = splineToGrindOn;
            splineFollower.RebuildImmediate();

            splineProjector.enabled = true;
            splineProjector.spline = splineToGrindOn;
            splineProjector.RebuildImmediate();
            splineProjector.CalculateProjection();

            double percent = splineProjector.GetPercent();
            splineFollower.SetPercent(percent);

            splineFollower.follow = true;
            if (splineFollower.result.percent < 0.5)
            {
                splineFollower.followSpeed = startSpeed;
            }
            else
            {
                splineFollower.followSpeed = -startSpeed;
            }
            isGrinding = true;
            OnRailGrindStateChange?.Invoke(isGrinding);
            return;

        }
    }

    public void JumpOffAtTheEndOfRail()
    {
        if (splineFollower.direction == Spline.Direction.Forward)
        {
            if (splineFollower.result.percent > 0.95f)
            {
                TriggerJumpingOffRails?.Invoke(JumpState.inAir);
            }
        }
        else
        {
            if (splineFollower.result.percent < 0.05f)
            {
                TriggerJumpingOffRails?.Invoke(JumpState.inAir);
            }
        }
    }

    void ExitRails()
    {
        splineFollower.follow = !splineFollower.follow;
        splineFollower.spline = null;
        splineProjector.spline = null;
        splineProjector.enabled = false;
        isGrinding = false;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        OnRailGrindStateChange?.Invoke(splineFollower.follow);
    }
    private void  GetOffRailsOnJump(JumpState _jumpStste)
    {
        if (_jumpStste == JumpState.Grounded || splineFollower.spline == null) return;
        ExitRails();
    }

    private void GetCurrentConnection(Node.Connection[] availableConnections)
    {
        foreach (var connection in availableConnections)
        {
            if (connection.spline == splineFollower.spline)
            {
                currentConnection = connection;
                break;
            }
        }
    }

    Vector3 JumpMomentumAddon()
    {
        if (isGrinding && isSpeedingUp)
        { return transform.forward *3f; }

        else if(isGrinding && !isSpeedingUp)
        { return transform.forward * 2.5f; }

        return Vector3.zero;
    }

    void disableInputEnabled()
    {
        isInputPressed = false;
    }

    void DisableSwitchingTrack()
    {
        isSwitchingTrack = false;
    }
}
