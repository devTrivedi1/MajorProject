using Dreamteck.Splines;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Diagnostics;


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

    public static Action<bool> PlayerIsNowGrinding;

    public bool isGrinding = false;
    bool isInputPressed = false;


    private void OnEnable()
    {
        splineFollower = GetComponent<SplineFollower>();
        splineProjector = GetComponent<SplineProjector>();
        startSpeed = splineFollower.followSpeed;
        Node[] nodeArray = FindObjectsOfType<Node>();
        AllNodes = nodeArray.ToList();
        SplineComputer[] splineArray = FindObjectsOfType<SplineComputer>();
        AllSplines = splineArray.ToList();
        splineFollower.onNode += OnNode;
    }

    private void OnDisable()
    {
        splineFollower.onNode -= OnNode;
    }

    private void Update()
    {
        // no spline reference, then no grinding

        if (splineFollower.spline == null) { splineFollower.follow = false; return; }


        // switching player direction 
        if (Input.GetKeyDown(KeyCode.W) && splineFollower.direction == Spline.Direction.Backward)
        {
            float value = Mathf.Abs(splineFollower.followSpeed);
            splineFollower.followSpeed = value;
        }
        if (Input.GetKeyDown(KeyCode.S) && splineFollower.direction == Spline.Direction.Forward)
        {
            float value = -splineFollower.followSpeed;
            splineFollower.followSpeed = value;
        }

        // getting of the rail 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            splineFollower.follow = !splineFollower.follow;
            splineFollower.spline = null;
            splineProjector.spline = null;
            isGrinding = false;
            transform.position = !splineFollower.follow ? splineFollower.result.position + (Vector3.right * 8) : splineFollower.result.position + (Vector3)splineFollower.motion.offset;
            PlayerIsNowGrinding?.Invoke(splineFollower.follow);

        }

        // Sprinting
        bool isSpeedingUp = Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = isSpeedingUp ? startSpeed * speedMultiplier : startSpeed;
        targetSpeed = splineFollower.direction == Spline.Direction.Forward ? targetSpeed : -targetSpeed;
        splineFollower.followSpeed = Mathf.Lerp(splineFollower.followSpeed, targetSpeed, transitionSpeed * Time.deltaTime);


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


                foreach (var connection in closestNode.GetConnections())
                {
                    if (connection.spline != splineFollower.spline)
                    {
                        isInputPressed = true;
                        Invoke(nameof(disableInputEnabled), 0.15f);
                        SwitchSpline(currentConnection, connection);
                        closestNode = null;
                        return;
                    }
                }
            }
        }

    }

    public void GoGrindOnThoseRails(SplineComputer splineToGrindOn)
    {
        if (!isGrinding)
        {
            splineFollower.spline = splineToGrindOn;
            splineFollower.RebuildImmediate();

            splineProjector.spline = splineToGrindOn;
            splineProjector.RebuildImmediate();
            splineProjector.CalculateProjection();

            double percent = splineProjector.GetPercent();
            splineFollower.SetPercent(percent);

            splineFollower.follow = true;
            splineFollower.followSpeed = startSpeed;
            isGrinding = true;
            PlayerIsNowGrinding?.Invoke(isGrinding);
            return;

        }
    }

    // this  will choose a default track if the current track is closed and no input is pressed
    private void OnNode(List<SplineTracer.NodeConnection> passed)
    {
        if(isInputPressed || !isGrinding) return;

        if (closestNode == null) return;
        Node.Connection[] availableConnections = new Node.Connection[closestNode.GetConnections().Length];
        closestNode.GetConnections().CopyTo(availableConnections, 0);

        foreach (var connection in availableConnections)
        {
            if (connection.spline == splineFollower.spline)
            {
                currentConnection = connection;
                break;
            }
        }

        foreach (var connection in closestNode.GetConnections())
        {

            if (!currentConnection.spline.isClosed && connection.spline != splineFollower.spline)
            {
                SwitchSpline(currentConnection, connection);
                closestNode = null;
                return;
            }
        }
    }




    void SwitchSpline(Node.Connection from, Node.Connection to)
    {

        splineFollower.spline = to.spline;
        splineFollower.RebuildImmediate();
        double startpercent = splineFollower.ClipPercent(to.spline.GetPointPercent(to.pointIndex));
        splineFollower.SetPercent(startpercent);
        if (to.spline.isClosed) return;
        if(splineFollower.result.percent < 0.5)
        {
            splineFollower.followSpeed = startSpeed;
        }
        else
        {
            splineFollower.followSpeed = -startSpeed;
        }

    }


    void disableInputEnabled()
    {
        isInputPressed = false;
    }
}
