using System;
using System.Collections.Generic;
using Mastercam.Database;
using Mastercam.Math;
using Mastercam.IO;
using Mastercam.Database.Types;
using Mastercam.GeometryUtility.Types;
using Mastercam.App.Types;
using Mastercam.GeometryUtility;
using Mastercam.Support;
using Mastercam.Curves;
using Mastercam.BasicGeometry;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace _CustomNethook
{
    public class CustomNethook : Mastercam.App.NetHook3App
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);

        const UInt32 MouseEventLeftDown = 0x0201;
        const UInt32 MouseEventLeftUp = 0x0202;
        public Mastercam.App.Types.MCamReturn CustomNethookRun(Mastercam.App.Types.MCamReturn notused)
        {
            var tempList1 = new List<int>(); // list of x+, y+ entities
            var tempList2 = new List<int>(); // list of x-, y- entitites
            var tempList3 = new List<int>(); // list of starting arcs
            var tempList4 = new List<int>(); // list of arc points
            var tempList5 = new List<Geometry>(); // list is broken arcs
            var tempList6 = new List<Geometry>(); // error list
            var tempList7 = new List<Geometry>(); // arcs to break
            var tempList8 = new List<int>(); // list of starting entities
            var tempList9 = new List<int>(); // list of all points
            var templist10 = new List<int>(); // temporarily holds geoID of chainEntities
            var templist80 = new List<int>();
            var templist81 = new List<int>();
            var templist500 = new List<int>();
            var anotherTemplist500 = new List<int>();
            var templist501 = new List<int>();
            var anotherTemplist501 = new List<int>();
            var pointList = new List<int>();
            var lineList = new List<int>();
            var finalPointList500 = new List<int>();
            var finalPointList501 = new List<int>();
            var secondPointList = new List<int>();
            var upperCreaseID = new List<int>();
            var lowerCreaseID = new List<int>();
            MCView Top = new MCView();
            var creaseX1 = 0.0;
            var creaseX2 = 0.0;
            var creaseY1 = 0.0;
            var creaseY2 = 0.0;
            var creaseX3 = 0.0;
            var creaseX4 = 0.0;
            var creaseY3 = 0.0;
            var creaseY4 = 0.0;
            int createdUpperCrease = 502;
            int createdLowerCrease = 503;
            var maleLandWidth = 0.0;
            var femaleLandWidth = 0.0;
            var overlap = 0.005;









            void translate()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(75);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown)
                {
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(75, true);
                LevelsManager.RefreshLevelsManager();
                var tempChain1 = SearchManager.SelectAllGeometryOnLevel(75, true);
                var tempSelectedGeometry1 = SearchManager.GetSelectedGeometry();
                var temp1 = GeometryManipulationManager.CopySelectedGeometryToLevel(10, true);
                SelectionManager.UnselectAllGeometry();
                var tempChain2 = SearchManager.SelectAllGeometryOnLevel(75, true);
                var tempSelectedGeometry2 = SearchManager.GetSelectedGeometry();
                var temp2 = GeometryManipulationManager.CopySelectedGeometryToLevel(11, true);
                Point3D startPoint = new Point3D(0.0, 0.0, 0.0);
                Point3D endPointUpper = new Point3D(-0.001, -0.001, 0.0);
                Point3D endPointLower = new Point3D(0.001, 0.001, 0.0);
                Point3D resultPoint = new Point3D(-0.001, 0.001, 0.0);
                foreach (var i in tempSelectedGeometry1)
                {
                    tempList3.Add(i.GetEntityID());
                }

                var temp1Geometry = SearchManager.SelectAllGeometryOnLevel(10, true);
                var tempSelectedGeometry1Result = SearchManager.GetSelectedGeometry();
                foreach (var entity in tempSelectedGeometry1Result)
                {
                    entity.Color = 10;
                    entity.Selected = false;
                    entity.Translate(startPoint, endPointUpper, Top, Top);
                    tempList1.Add(entity.GetEntityID());
                    entity.Commit();
                }
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                SelectionManager.UnselectAllGeometry();


                var temp2Geometry = SearchManager.SelectAllGeometryOnLevel(11, true);
                var tempSelectedGeometry2Result = SearchManager.GetSelectedGeometry();
                foreach (var entity in tempSelectedGeometry2Result)
                {
                    entity.Color = 11;
                    entity.Selected = false;
                    entity.Translate(startPoint, endPointLower, Top, Top);
                    tempList2.Add(entity.GetEntityID());
                    entity.Commit();
                }
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                SelectionManager.UnselectAllGeometry();

                tempSelectedGeometry1Result = null;
                tempSelectedGeometry1 = null;
                tempSelectedGeometry2Result = null;
                tempSelectedGeometry2 = null;

            } // Moves line in X+Y+ direction and in X-Y- direction
            void findIntersectionOfArcs()
            {
                SelectionManager.UnselectAllGeometry();
                foreach (var v in tempList1)
                {
                    var ArcID = Geometry.RetrieveEntity(v);
                    if (ArcID is ArcGeometry arc)
                    {
                        arc = (ArcGeometry)ArcID;
                        var arc1X1 = arc.EndPoint1.x; // Arc 1 x 1
                        var arc1X2 = arc.EndPoint2.x; // Arc 1 x 2
                        var arc1Y1 = arc.EndPoint1.y; // Arc 1 y 1
                        var arc1Y2 = arc.EndPoint2.y; // Arc 1 y 2
                        var arc1Center = arc.Data.CenterPoint;
                        Point3D arc1StartPoint = new Point3D(arc1X1, arc1Y1, 0.0);
                        var arc1Rad = VectorManager.Distance(arc1Center, arc1StartPoint);
                        var arcStartDegrees = arc.Data.StartAngleDegrees;
                        var arcEndDegrees = arc.Data.EndAngleDegrees;
                        foreach (var u in tempList2)
                        {
                            var secondArcID = Geometry.RetrieveEntity(u);
                            if (secondArcID is ArcGeometry secondArc)
                            {
                                secondArc = (ArcGeometry)secondArcID;
                                var arc2X1 = secondArc.EndPoint1.x; // Arc 2 x 1
                                var arc2X2 = secondArc.EndPoint2.x; // Arc 2 x 2
                                var arc2Y1 = secondArc.EndPoint1.y; // Arc 2 y 1
                                var arc2Y2 = secondArc.EndPoint2.y; // Arc 2 y 2
                                var arc2Center = secondArc.Data.CenterPoint;
                                Point3D arc2StartPoint = new Point3D(arc2X1, arc2Y1, 0.0);
                                var arc2Rad = VectorManager.Distance(arc2Center, arc2StartPoint);
                                var arc2StartDegrees = secondArc.Data.StartAngleDegrees;
                                var arc2EndDegrees = secondArc.Data.EndAngleDegrees;
                                var C1C2 = VectorManager.Distance(arc1Center, arc2Center);
                                if (C1C2 < arc1Rad + arc2Rad)
                                {
                                    var x1 = arc1Center.x;
                                    var x2 = arc2Center.x;
                                    var y1 = arc1Center.y;
                                    var y2 = arc2Center.y;
                                    var r1 = arc1Rad;
                                    var r2 = arc2Rad;
                                    var a = (((r1 * r1) - (r2 * r2) + (C1C2 * C1C2)) / (2 * C1C2));
                                    var b = (((r2 * r2) - (r1 * r1) + (C1C2 * C1C2)) / (2 * C1C2));
                                    var h = Math.Sqrt((r2 * r2) - (a * a));
                                    var x5 = (x1 + (a / C1C2) * (x2 - x1));
                                    var y5 = (y1 + (a / C1C2) * (y2 - y1));
                                    var x3 = (x5 - (h * (y2 - y1)) / C1C2);
                                    var y3 = (y5 + (h * (x2 - x1)) / C1C2);
                                    var x4 = (x5 + (h * (y2 - y1)) / C1C2);
                                    var y4 = (y5 - (h * (x2 - x1)) / C1C2);
                                    Point3D pt3 = new Point3D(x3, y3, 0.0);
                                    Point3D pt4 = new Point3D(x4, y4, 0.0);
                                    var delta3Xneg = (x2 - pt3.x);
                                    var delta3Yneg = (y2 - pt3.y);
                                    var delta4Xneg = (x2 - pt4.x);
                                    var delta4Yneg = (y2 - pt4.y);
                                    var delta3Radneg = Math.Atan2(delta3Xneg, delta3Yneg);
                                    var delta4Radneg = Math.Atan2(delta4Xneg, delta4Yneg);
                                    var pt3degreeNeg = VectorManager.RadiansToDegrees(delta3Radneg);
                                    var pt4degreeNeg = VectorManager.RadiansToDegrees(delta4Radneg);
                                    if (pt3degreeNeg < 0.0)
                                    {
                                        pt3degreeNeg = (pt3degreeNeg + 360);
                                    }
                                    if (pt4degreeNeg < 0.0)
                                    {
                                        pt4degreeNeg = (pt4degreeNeg + 360);
                                    }
                                    if (((arc2StartDegrees < pt3degreeNeg) && (pt3degreeNeg < arc2EndDegrees)) || ((pt3degreeNeg < arc2StartDegrees) && (arc2EndDegrees < pt3degreeNeg)))
                                    {
                                        var point3 = new Mastercam.BasicGeometry.PointGeometry(pt3);
                                        point3.Color = 94;
                                        point3.Level = 94;
                                        point3.Commit();
                                        tempList4.Add(point3.GetEntityID());
                                        tempList9.Add(point3.GetEntityID());
                                    }
                                    if (((arc2StartDegrees < pt4degreeNeg) && (pt4degreeNeg < arc2EndDegrees)) || ((pt4degreeNeg < arc2StartDegrees) && (arc2EndDegrees < pt4degreeNeg)))
                                    {
                                        var point4 = new Mastercam.BasicGeometry.PointGeometry(pt4);
                                        point4.Color = 94;
                                        point4.Level = 94;
                                        point4.Commit();
                                        tempList9.Add(point4.GetEntityID());
                                    }
                                }
                            }
                        }
                    }
                }
            } // Finds intersections of arcs
            void findIntersectionOfLines()
            {
                SelectionManager.UnselectAllGeometry();
                Point3D startPoint = new Point3D(0.0, 0.0, 0.0);
                Point3D resultPointUp = new Point3D(-0.001, 0.001, 0.0);
                Point3D resultPointDown = new Point3D(0.001, -0.001, 0.0);
                foreach (var v in tempList1)
                {
                    var lineID = Geometry.RetrieveEntity(v);
                    if (lineID is LineGeometry line)
                    {
                        line = (LineGeometry)lineID;
                        var line1X1 = line.EndPoint1.x; // Line 1 x 1
                        var line1X2 = line.EndPoint2.x; // Line 1 x 2
                        var line1Y1 = line.EndPoint1.y; // Line 1 y 1
                        var line1Y2 = line.EndPoint2.y; // Line 1 y 2
                        foreach (var u in tempList2)
                        {
                            var secondLineID = Geometry.RetrieveEntity(u);
                            if (secondLineID is LineGeometry secondLine)
                            {
                                secondLine = (LineGeometry)secondLineID;
                                var line2X1 = secondLine.EndPoint1.x; // Line 2 x 1
                                var line2X2 = secondLine.EndPoint2.x; // Line 2 x 2
                                var line2Y1 = secondLine.EndPoint1.y; // Line 2 y 1
                                var line2Y2 = secondLine.EndPoint2.y; // Line 2 y 2
                                var slope1 = (line1Y2 - line1Y1) / (line1X2 - line1X1); // check slope of line 1
                                var slope2 = (line2Y2 - line2Y1) / (line2X2 - line2X1); // check slope of line 2
                                if (slope1 != slope2)
                                { // if slopes dont match lines are not parallel
                                    // calculates intersections
                                    var a1 = line1Y2 - line1Y1;
                                    var b1 = line1X1 - line1X2;
                                    var c1 = a1 * line1X1 + b1 * line1Y1;
                                    var a2 = line2Y2 - line2Y1;
                                    var b2 = line2X1 - line2X2;
                                    var c2 = a2 * line2X1 + b2 * line2Y1;
                                    var delta = a1 * b2 - a2 * b1;
                                    Point3D pt1 = new Point3D((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta, 0.0); // point of intersection
                                    var AB = Math.Sqrt((line1X2 - line1X1) * (line1X2 - line1X1) + (line1Y2 - line1Y1) * (line1Y2 - line1Y1));
                                    var AP = Math.Sqrt((pt1.x - line1X1) * (pt1.x - line1X1) + (pt1.y - line1Y1) * (pt1.y - line1Y1));
                                    var PB = Math.Sqrt((line1X2 - pt1.x) * (line1X2 - pt1.x) + (line1Y2 - pt1.y) * (line1Y2 - pt1.y));
                                    var AB2 = Math.Sqrt((line2X2 - line2X1) * (line2X2 - line2X1) + (line2Y2 - line2Y1) * (line2Y2 - line2Y1));
                                    var AP2 = Math.Sqrt((pt1.x - line2X1) * (pt1.x - line2X1) + (pt1.y - line2Y1) * (pt1.y - line2Y1));
                                    var PB2 = Math.Sqrt((line2X2 - pt1.x) * (line2X2 - pt1.x) + (line2Y2 - pt1.y) * (line2Y2 - pt1.y));
                                    if (AB == AP + PB)
                                    { // if point on line 1
                                        if (AB2 == AP2 + PB2)
                                        { // if point on line 2
                                            if (line1X1 <= pt1.x && pt1.x <= line2X1)
                                            { // if needs to shift up and left
                                                var newPoint = new Mastercam.BasicGeometry.PointGeometry(pt1);
                                                newPoint.Color = 94;
                                                newPoint.Level = 94;
                                                newPoint.Translate(startPoint, resultPointUp, Top, Top);
                                                newPoint.Commit();
                                                tempList9.Add(newPoint.GetEntityID());
                                                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                            }
                                            if (line1X1 >= pt1.x && pt1.x >= line2X1)
                                            { // if needs to shift down and right
                                                var newPoint = new Mastercam.BasicGeometry.PointGeometry(pt1);
                                                newPoint.Color = 94;
                                                newPoint.Level = 94;
                                                newPoint.Translate(startPoint, resultPointDown, Top, Top);
                                                newPoint.Commit();
                                                tempList9.Add(newPoint.GetEntityID());
                                                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } // Finds intersections of lines
            void clearTempLinesAndArcs()
            {
                foreach (var i in tempList1)
                {
                    var tempI = Geometry.RetrieveEntity(i);
                    tempI.Delete();
                }
                foreach (var i in tempList2)
                {
                    var tempI = Geometry.RetrieveEntity(i);
                    tempI.Delete();
                }
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                //GraphicsManager.Repaint(true);
            } // Deletes temporary lines and arcs
            void breakArcsWithPoints()
            {
                foreach (var i in tempList3)
                {
                    var ArcID = Geometry.RetrieveEntity(i);
                    if (ArcID is ArcGeometry arc)
                    {
                        var arc1X1 = arc.EndPoint1.x; // Arc x 1
                        var arc1X2 = arc.EndPoint2.x; // Arc x 2
                        var arc1Y1 = arc.EndPoint1.y; // Arc y 1
                        var arc1Y2 = arc.EndPoint2.y; // Arc y 2
                        var arc1Center = arc.Data.CenterPoint;
                        Point3D arc1StartPoint = new Point3D(arc1X1, arc1Y1, 0.0);
                        Point3D arc1EndPoint = new Point3D(arc1X2, arc1Y2, 0.0);
                        var tolerance = 0.001;
                        foreach (var v in tempList4)
                        {
                            var u = PointGeometry.RetrieveEntity(v);
                            if (u is PointGeometry point)
                            {
                                var ux = point.Data.x;
                                var uy = point.Data.y;
                                Point3D uPoint = new Point3D(uy, ux, 0.0);
                                var delta3Xneg = (arc1Center.x - uPoint.x);
                                var delta3Yneg = (arc1Center.y - uPoint.y);
                                var delta3Radneg = Math.Atan2(delta3Xneg, delta3Yneg);
                                var pt3degreeNeg = VectorManager.RadiansToDegrees(delta3Radneg);
                                if (pt3degreeNeg < 0.0)
                                {
                                    pt3degreeNeg = (pt3degreeNeg + 360);
                                }
                                var startToUDist = Math.Abs(VectorManager.Distance(arc1StartPoint, uPoint));
                                var endToUDist = Math.Abs(VectorManager.Distance(arc1EndPoint, uPoint));
                                if ((startToUDist >= (endToUDist - tolerance)) && (startToUDist <= (endToUDist + tolerance)))
                                {
                                    BreakManyPiecesParameters broken = new BreakManyPiecesParameters()
                                    {
                                        Curves = true,
                                        Number = 2,
                                        Method = 0
                                    };
                                    tempList7.Add(arc);
                                    GeometryManipulationManager.BreakManyPieces(tempList7, broken, ref tempList5, ref tempList6);
                                }
                            }
                        }
                    }
                    ArcID = null;
                }
            } // Breaks arcs where intersections were
            void selectNewGeo()
            {
                SelectionManager.UnselectAllGeometry();
                SearchManager.SelectAllGeometryOnLevel(75, true);
                var tempSelectedGeometry1 = SearchManager.GetSelectedGeometry();
                foreach (var i in tempSelectedGeometry1)
                {
                    i.Selected = false;
                    tempList8.Add(i.GetEntityID());
                }
                tempSelectedGeometry1 = null;
                SelectionManager.UnselectAllGeometry();
            } // Selects all geometry (there are new entities)
            void colorCrossovers()
            {
                var tolerance = .005;
                var arcX1 = 0.0;
                var arcX2 = 0.0;
                var arcY1 = 0.0;
                var arcY2 = 0.0;
                Point3D arcStartPoint = new Point3D(arcX1, arcY1, 0.0);
                Point3D arcEndPoint = new Point3D(arcX2, arcY2, 0.0);
                var lineX1 = 0.0;
                var lineX2 = 0.0;
                var lineY1 = 0.0;
                var lineY2 = 0.0;
                Point3D lineStartPoint = new Point3D(lineX1, lineY1, 0.0);
                Point3D lineEndPoint = new Point3D(lineX2, lineY2, 0.0);
                var nextArcX1 = 0.0;
                var nextArcX2 = 0.0;
                var nextArcY1 = 0.0;
                var nextArcY2 = 0.0;
                Point3D nextArcStartPoint = new Point3D(nextArcX1, nextArcY1, 0.0);
                Point3D nextArcEndPoint = new Point3D(nextArcX2, nextArcY2, 0.0);
                var nextLineX1 = 0.0;
                var nextLineX2 = 0.0;
                var nextLineY1 = 0.0;
                var nextLineY2 = 0.0;
                Point3D nextLineStartPoint = new Point3D(nextLineX1, nextLineY1, 0.0);
                Point3D nextLineEndPoint = new Point3D(nextLineX2, nextLineY2, 0.0);
                var firstEntity = 0;
                var step = 0;
                var chainDetails = new Mastercam.Database.Interop.ChainDetails();// Preps the ChainDetails plugin
                var selectedChains = ChainManager.ChainAll(75);// Chains all geometry on level 75
                var chainDirection = ChainDirectionType.CounterClockwise;// Going to be used to make sure all chains go the same direction
                ChainManager.StartChainAtLongest(selectedChains);
                foreach (var chain in selectedChains)
                {
                    var chainData = chainDetails.GetData(chain);
                    var chainEntityList = chainData.ChainEntities;
                    foreach (var t in chainEntityList)
                    {
                        (t.Key).Color = 9;
                        (t.Key).Commit();
                    }
                }
                foreach (var chain in selectedChains)
                {
                    templist10.Clear();
                    step = 0;
                    chain.Direction = chainDirection;
                    var chainEntity = ChainManager.GetGeometryInChain(chain);
                stepStart:
                    if (step == 0)
                    {
                        foreach (var entity in chainEntity)
                        {
                            if (entity is ArcGeometry arc && step == 0)
                            {
                                arcX1 = arc.EndPoint1.x; // Arc 1 x 1
                                arcX2 = arc.EndPoint2.x; // Arc 1 x 2
                                arcY1 = arc.EndPoint1.y; // Arc 1 y 1
                                arcY2 = arc.EndPoint2.y; // Arc 1 y 2
                                arcStartPoint = new Point3D(arcX1, arcY1, 0.0); // Arc 1 start point
                                arcEndPoint = new Point3D(arcX2, arcY2, 0.0); // Arc 1 end point
                                firstEntity = 1; // Saves that first entity is Arc
                                foreach (var v in tempList9)
                                {
                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                    if (w is PointGeometry point && step == 0)
                                    {
                                        var ux = point.Data.x;// X location of point
                                        var uy = point.Data.y;// Y location of point
                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                        var arcStartDistance = Math.Abs(VectorManager.Distance(uPoint, arcStartPoint));// Distance between entity start point and crossover point
                                        var arcEndDistance = Math.Abs(VectorManager.Distance(uPoint, arcEndPoint));// Distance between entity end point and crossover point
                                        if (arcStartDistance <= tolerance && step == 0)
                                        {
                                            entity.Color = 10;// Changes color of entity to 10
                                            entity.Commit();// Saves entity into Mastercam Database
                                            step = 1;// Sets step to 1
                                            templist10.Add(entity.GetEntityID());
                                        }
                                    }
                                }
                            }
                            if (entity is LineGeometry line && step == 0)
                            {
                                lineX1 = line.EndPoint1.x; // line 1 x 1
                                lineX2 = line.EndPoint2.x; // line 1 x 2
                                lineY1 = line.EndPoint1.y; // line 1 y 1
                                lineY2 = line.EndPoint2.y; // line 1 y 2
                                lineStartPoint = new Point3D(lineX1, lineY1, 0.0); // Line 1 start point
                                lineEndPoint = new Point3D(lineX2, lineY2, 0.0); // Line 1 end point
                                firstEntity = 2; // Saves that first entity is Line
                                foreach (var v in tempList9)
                                {
                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                    if (w is PointGeometry point && step == 0)
                                    {
                                        var ux = point.Data.x;// X location of point
                                        var uy = point.Data.y;// Y location of point
                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                        var lineStartDistance = Math.Abs(VectorManager.Distance(uPoint, lineStartPoint));// Distance between entity start point and crossover point
                                        var lineEndDistance = Math.Abs(VectorManager.Distance(uPoint, lineEndPoint));// Distance between entity end point and crossover point
                                        if (lineStartDistance <= tolerance && step == 0)
                                        {
                                            entity.Color = 10;// Changes color of entity to 10
                                            entity.Commit();// Saves entity into Mastercam Database
                                            step = 1;// Sets step to 1
                                            templist10.Add(entity.GetEntityID());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (step == 1)
                    {
                        foreach (var entity in chainEntity)
                        {
                            if (firstEntity == 1 && step == 1)
                            {
                                var thisEntity = ArcGeometry.RetrieveEntity(templist10[0]);
                                if (thisEntity is ArcGeometry arc && step == 1)
                                {
                                    arcX1 = arc.EndPoint1.x; // Arc 1 x 1
                                    arcX2 = arc.EndPoint2.x; // Arc 1 x 2
                                    arcY1 = arc.EndPoint1.y; // Arc 1 y 1
                                    arcY2 = arc.EndPoint2.y; // Arc 1 y 2
                                    arcStartPoint = new Point3D(arcX1, arcY1, 0.0); // Arc 1 start point
                                    arcEndPoint = new Point3D(arcX2, arcY2, 0.0); // Arc 1 end point
                                    if (entity is ArcGeometry nextArc && (entity.GetEntityID() != templist10[0]) && step == 1)
                                    {
                                        nextArcX1 = nextArc.EndPoint1.x;
                                        nextArcX2 = nextArc.EndPoint2.x;
                                        nextArcY1 = nextArc.EndPoint1.y;
                                        nextArcY2 = nextArc.EndPoint2.y;
                                        nextArcStartPoint = new Point3D(nextArcX1, nextArcY1, 0.0); // Arc 2 start point
                                        nextArcEndPoint = new Point3D(nextArcX2, nextArcY2, 0.0); // Arc 2 end point
                                        var entityStartDistance = VectorManager.Distance(arcEndPoint, nextArcStartPoint);
                                        var entityEndDistance = VectorManager.Distance(arcEndPoint, nextArcEndPoint);
                                        if (entityStartDistance <= tolerance && step == 1)
                                        {
                                            entity.Color = 10;
                                            entity.Commit();
                                            templist10.Clear();
                                            templist10.Add(entity.GetEntityID());
                                            step = 2;
                                            firstEntity = 1;
                                            foreach (var v in tempList9)
                                            {
                                                var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                if (w is PointGeometry point)
                                                {
                                                    var ux = point.Data.x;// X location of point
                                                    var uy = point.Data.y;// Y location of point
                                                    Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                    var entityPointDistance = VectorManager.Distance(uPoint, nextArcEndPoint);
                                                    if (entityPointDistance <= tolerance)
                                                    {
                                                        step = 3;
                                                    }
                                                }
                                            }
                                        }
                                        if (entityEndDistance <= tolerance && step == 1)
                                        {
                                            entity.Color = 10;
                                            entity.Commit();
                                            templist10.Clear();
                                            templist10.Add(entity.GetEntityID());
                                            step = 4;
                                            firstEntity = 1;
                                            foreach (var v in tempList9)
                                            {
                                                var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                if (w is PointGeometry point)
                                                {
                                                    var ux = point.Data.x;// X location of point
                                                    var uy = point.Data.y;// Y location of point
                                                    Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                    var entityPointDistance = VectorManager.Distance(uPoint, nextArcStartPoint);
                                                    if (entityPointDistance <= tolerance)
                                                    {
                                                        step = 5;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (entity is LineGeometry nextLine && (entity.GetEntityID() != templist10[0]) && step == 1)
                                    {
                                        nextLineX1 = nextLine.EndPoint1.x;
                                        nextLineX2 = nextLine.EndPoint2.x;
                                        nextLineY1 = nextLine.EndPoint1.y;
                                        nextLineY2 = nextLine.EndPoint2.y;
                                        nextLineStartPoint = new Point3D(nextLineX1, nextLineY1, 0.0); // Arc 2 start point
                                        nextLineEndPoint = new Point3D(nextLineX2, nextLineY2, 0.0); // Arc 2 end point
                                        var entityStartDistance = VectorManager.Distance(arcEndPoint, nextLineStartPoint);
                                        var entityEndDistance = VectorManager.Distance(arcEndPoint, nextLineEndPoint);
                                        if (entityStartDistance <= tolerance && step == 1)
                                        {
                                            entity.Color = 10;
                                            entity.Commit();
                                            templist10.Clear();
                                            templist10.Add(entity.GetEntityID());
                                            step = 2;
                                            firstEntity = 2;
                                            foreach (var v in tempList9)
                                            {
                                                var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                if (w is PointGeometry point)
                                                {
                                                    var ux = point.Data.x;// X location of point
                                                    var uy = point.Data.y;// Y location of point
                                                    Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                    var entityPointDistance = VectorManager.Distance(uPoint, nextLineEndPoint);
                                                    if (entityPointDistance <= tolerance)
                                                    {
                                                        step = 3;
                                                    }
                                                }
                                            }
                                        }
                                        if (entityEndDistance <= tolerance && step == 1)
                                        {
                                            entity.Color = 10;
                                            entity.Commit();
                                            templist10.Clear();
                                            templist10.Add(entity.GetEntityID());
                                            step = 4;
                                            firstEntity = 2;
                                            foreach (var v in tempList9)
                                            {
                                                var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                if (w is PointGeometry point)
                                                {
                                                    var ux = point.Data.x;// X location of point
                                                    var uy = point.Data.y;// Y location of point
                                                    Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                    var entityPointDistance = VectorManager.Distance(uPoint, nextLineStartPoint);
                                                    if (entityPointDistance <= tolerance)
                                                    {
                                                        step = 5;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (firstEntity == 2 && step == 1)
                            {

                                var thisEntity = LineGeometry.RetrieveEntity(templist10[0]);
                                if (thisEntity is LineGeometry line && step == 1)
                                {
                                    lineX1 = line.EndPoint1.x; // Arc 1 x 1
                                    lineX2 = line.EndPoint2.x; // Arc 1 x 2
                                    lineY1 = line.EndPoint1.y; // Arc 1 y 1
                                    lineY2 = line.EndPoint2.y; // Arc 1 y 2
                                    lineStartPoint = new Point3D(lineX1, lineY1, 0.0); // Arc 1 start point
                                    lineEndPoint = new Point3D(lineX2, lineY2, 0.0); // Arc 1 end point
                                    if (entity is ArcGeometry nextArc && (entity.GetEntityID() != templist10[0]) && step == 1)
                                    {
                                        nextArcX1 = nextArc.EndPoint1.x;
                                        nextArcX2 = nextArc.EndPoint2.x;
                                        nextArcY1 = nextArc.EndPoint1.y;
                                        nextArcY2 = nextArc.EndPoint2.y;
                                        nextArcStartPoint = new Point3D(nextArcX1, nextArcY1, 0.0); // Arc 2 start point
                                        nextArcEndPoint = new Point3D(nextArcX2, nextArcY2, 0.0); // Arc 2 end point
                                        var entityStartDistance = VectorManager.Distance(lineEndPoint, nextArcStartPoint);
                                        var entityEndDistance = VectorManager.Distance(lineEndPoint, nextArcEndPoint);
                                        if (entityStartDistance <= tolerance && step == 1)
                                        {
                                            entity.Color = 10;
                                            entity.Commit();
                                            templist10.Clear();
                                            templist10.Add(entity.GetEntityID());
                                            step = 2;
                                            firstEntity = 1;
                                            foreach (var v in tempList9)
                                            {
                                                var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                if (w is PointGeometry point)
                                                {
                                                    var ux = point.Data.x;// X location of point
                                                    var uy = point.Data.y;// Y location of point
                                                    Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                    var entityPointDistance = VectorManager.Distance(uPoint, nextArcEndPoint);
                                                    if (entityPointDistance <= tolerance)
                                                    {
                                                        step = 3;
                                                    }
                                                }
                                            }
                                        }
                                        if (entityEndDistance <= tolerance && step == 1)
                                        {
                                            entity.Color = 10;
                                            entity.Commit();
                                            templist10.Clear();
                                            templist10.Add(entity.GetEntityID());
                                            step = 4;
                                            firstEntity = 1;
                                            foreach (var v in tempList9)
                                            {
                                                var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                if (w is PointGeometry point)
                                                {
                                                    var ux = point.Data.x;// X location of point
                                                    var uy = point.Data.y;// Y location of point
                                                    Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                    var entityPointDistance = VectorManager.Distance(uPoint, nextArcStartPoint);
                                                    if (entityPointDistance <= tolerance)
                                                    {
                                                        step = 5;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (entity is LineGeometry nextLine && (entity.GetEntityID() != templist10[0]) && step == 1)
                                    {
                                        nextLineX1 = nextLine.EndPoint1.x;
                                        nextLineX2 = nextLine.EndPoint2.x;
                                        nextLineY1 = nextLine.EndPoint1.y;
                                        nextLineY2 = nextLine.EndPoint2.y;
                                        nextLineStartPoint = new Point3D(nextLineX1, nextLineY1, 0.0); // Arc 2 start point
                                        nextLineEndPoint = new Point3D(nextLineX2, nextLineY2, 0.0); // Arc 2 end point
                                        var entityStartDistance = VectorManager.Distance(lineEndPoint, nextLineStartPoint);
                                        var entityEndDistance = VectorManager.Distance(lineEndPoint, nextLineEndPoint);
                                        if (entityStartDistance <= tolerance && step == 1)
                                        {
                                            entity.Color = 10;
                                            entity.Commit();
                                            templist10.Clear();
                                            templist10.Add(entity.GetEntityID());
                                            step = 2;
                                            firstEntity = 2;
                                            foreach (var v in tempList9)
                                            {
                                                var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                if (w is PointGeometry point)
                                                {
                                                    var ux = point.Data.x;// X location of point
                                                    var uy = point.Data.y;// Y location of point
                                                    Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                    var entityPointDistance = VectorManager.Distance(uPoint, nextLineEndPoint);
                                                    if (entityPointDistance <= tolerance)
                                                    {
                                                        step = 3;
                                                    }
                                                }
                                            }
                                        }
                                        if (entityEndDistance <= tolerance && step == 1)
                                        {
                                            entity.Color = 10;
                                            entity.Commit();
                                            templist10.Clear();
                                            templist10.Add(entity.GetEntityID());
                                            step = 4;
                                            firstEntity = 2;
                                            foreach (var v in tempList9)
                                            {
                                                var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                if (w is PointGeometry point)
                                                {
                                                    var ux = point.Data.x;// X location of point
                                                    var uy = point.Data.y;// Y location of point
                                                    Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                    var entityPointDistance = VectorManager.Distance(uPoint, nextLineStartPoint);
                                                    if (entityPointDistance <= tolerance)
                                                    {
                                                        step = 5;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                    if (step == 2)
                    {
                        foreach (var entity in chainEntity)
                        {
                            if (entity.Color != 10 || entity.Color != 11)
                            {
                                if (firstEntity == 1 && step == 2)
                                {
                                    var thisEntity = ArcGeometry.RetrieveEntity(templist10[0]);
                                    if (thisEntity is ArcGeometry arc && step == 2)
                                    {
                                        arcX1 = arc.EndPoint1.x; // Arc 1 x 1
                                        arcX2 = arc.EndPoint2.x; // Arc 1 x 2
                                        arcY1 = arc.EndPoint1.y; // Arc 1 y 1
                                        arcY2 = arc.EndPoint2.y; // Arc 1 y 2
                                        arcStartPoint = new Point3D(arcX1, arcY1, 0.0); // Arc 1 start point
                                        arcEndPoint = new Point3D(arcX2, arcY2, 0.0); // Arc 1 end point
                                        if (entity is ArcGeometry nextArc && (entity.Color != 10) && (entity.Color != 11) && step == 2)
                                        {
                                            nextArcX1 = nextArc.EndPoint1.x;
                                            nextArcX2 = nextArc.EndPoint2.x;
                                            nextArcY1 = nextArc.EndPoint1.y;
                                            nextArcY2 = nextArc.EndPoint2.y;
                                            nextArcStartPoint = new Point3D(nextArcX1, nextArcY1, 0.0); // Arc 2 start point
                                            nextArcEndPoint = new Point3D(nextArcX2, nextArcY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(arcEndPoint, nextArcStartPoint);
                                            var entityEndDistance = VectorManager.Distance(arcEndPoint, nextArcEndPoint);
                                            if (entityStartDistance <= tolerance && step == 2)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 2;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 3;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 2)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 4;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 5;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                        if (entity is LineGeometry nextLine && (entity.Color != 10) && (entity.Color != 11) && step == 2)
                                        {
                                            nextLineX1 = nextLine.EndPoint1.x;
                                            nextLineX2 = nextLine.EndPoint2.x;
                                            nextLineY1 = nextLine.EndPoint1.y;
                                            nextLineY2 = nextLine.EndPoint2.y;
                                            nextLineStartPoint = new Point3D(nextLineX1, nextLineY1, 0.0); // Arc 2 start point
                                            nextLineEndPoint = new Point3D(nextLineX2, nextLineY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(arcEndPoint, nextLineStartPoint);
                                            var entityEndDistance = VectorManager.Distance(arcEndPoint, nextLineEndPoint);
                                            if (entityStartDistance <= tolerance && step == 2)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 2;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 3;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 2)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 4;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 5;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                    }
                                }
                                if (firstEntity == 2 && step == 2)
                                {
                                    var thisEntity = LineGeometry.RetrieveEntity(templist10[0]);
                                    if (thisEntity is LineGeometry line && step == 2)
                                    {
                                        lineX1 = line.EndPoint1.x; // Arc 1 x 1
                                        lineX2 = line.EndPoint2.x; // Arc 1 x 2
                                        lineY1 = line.EndPoint1.y; // Arc 1 y 1
                                        lineY2 = line.EndPoint2.y; // Arc 1 y 2
                                        lineStartPoint = new Point3D(lineX1, lineY1, 0.0); // Arc 1 start point
                                        lineEndPoint = new Point3D(lineX2, lineY2, 0.0); // Arc 1 end point
                                        if (entity is ArcGeometry nextArc && (entity.Color != 10) && (entity.Color != 11) && step == 2)
                                        {
                                            nextArcX1 = nextArc.EndPoint1.x;
                                            nextArcX2 = nextArc.EndPoint2.x;
                                            nextArcY1 = nextArc.EndPoint1.y;
                                            nextArcY2 = nextArc.EndPoint2.y;
                                            nextArcStartPoint = new Point3D(nextArcX1, nextArcY1, 0.0); // Arc 2 start point
                                            nextArcEndPoint = new Point3D(nextArcX2, nextArcY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(lineEndPoint, nextArcStartPoint);
                                            var entityEndDistance = VectorManager.Distance(lineEndPoint, nextArcEndPoint);
                                            if (entityStartDistance <= tolerance && step == 2)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 2;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 3;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 2)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 4;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 5;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                        if (entity is LineGeometry nextLine && (entity.Color != 10) && (entity.Color != 11) && step == 2)
                                        {
                                            nextLineX1 = nextLine.EndPoint1.x;
                                            nextLineX2 = nextLine.EndPoint2.x;
                                            nextLineY1 = nextLine.EndPoint1.y;
                                            nextLineY2 = nextLine.EndPoint2.y;
                                            nextLineStartPoint = new Point3D(nextLineX1, nextLineY1, 0.0); // Arc 2 start point
                                            nextLineEndPoint = new Point3D(nextLineX2, nextLineY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(lineEndPoint, nextLineStartPoint);
                                            var entityEndDistance = VectorManager.Distance(lineEndPoint, nextLineEndPoint);
                                            if (entityStartDistance <= tolerance && step == 2)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 2;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 3;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 2)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 4;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 5;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    } // Gets Geo that connects first entity end point to second entity start point (offset 2)
                    if (step == 3)
                    {
                        foreach (var entity in chainEntity)
                        {
                            if (entity.Color != 10 || entity.Color != 11)
                            {
                                if (firstEntity == 1 && step == 3)
                                {
                                    var thisEntity = ArcGeometry.RetrieveEntity(templist10[0]);
                                    if (thisEntity is ArcGeometry arc && step == 3)
                                    {
                                        arcX1 = arc.EndPoint1.x; // Arc 1 x 1
                                        arcX2 = arc.EndPoint2.x; // Arc 1 x 2
                                        arcY1 = arc.EndPoint1.y; // Arc 1 y 1
                                        arcY2 = arc.EndPoint2.y; // Arc 1 y 2
                                        arcStartPoint = new Point3D(arcX1, arcY1, 0.0); // Arc 1 start point
                                        arcEndPoint = new Point3D(arcX2, arcY2, 0.0); // Arc 1 end point
                                        if (entity is ArcGeometry nextArc && (entity.Color != 10) && (entity.Color != 11) && step == 3)
                                        {
                                            nextArcX1 = nextArc.EndPoint1.x;
                                            nextArcX2 = nextArc.EndPoint2.x;
                                            nextArcY1 = nextArc.EndPoint1.y;
                                            nextArcY2 = nextArc.EndPoint2.y;
                                            nextArcStartPoint = new Point3D(nextArcX1, nextArcY1, 0.0); // Arc 2 start point
                                            nextArcEndPoint = new Point3D(nextArcX2, nextArcY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(arcEndPoint, nextArcStartPoint);
                                            var entityEndDistance = VectorManager.Distance(arcEndPoint, nextArcEndPoint);
                                            if (entityStartDistance <= tolerance && step == 3)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 3;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 2;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 3)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 5;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 4;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                        if (entity is LineGeometry nextLine && (entity.Color != 10) && (entity.Color != 11) && step == 3)
                                        {
                                            nextLineX1 = nextLine.EndPoint1.x;
                                            nextLineX2 = nextLine.EndPoint2.x;
                                            nextLineY1 = nextLine.EndPoint1.y;
                                            nextLineY2 = nextLine.EndPoint2.y;
                                            nextLineStartPoint = new Point3D(nextLineX1, nextLineY1, 0.0); // Arc 2 start point
                                            nextLineEndPoint = new Point3D(nextLineX2, nextLineY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(arcEndPoint, nextLineStartPoint);
                                            var entityEndDistance = VectorManager.Distance(arcEndPoint, nextLineEndPoint);
                                            if (entityStartDistance <= tolerance && step == 3)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 3;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 2;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 3)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 5;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 4;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                    }
                                }
                                if (firstEntity == 2 && step == 3)
                                {
                                    var thisEntity = LineGeometry.RetrieveEntity(templist10[0]);
                                    if (thisEntity is LineGeometry line && step == 3)
                                    {
                                        lineX1 = line.EndPoint1.x; // Arc 1 x 1
                                        lineX2 = line.EndPoint2.x; // Arc 1 x 2
                                        lineY1 = line.EndPoint1.y; // Arc 1 y 1
                                        lineY2 = line.EndPoint2.y; // Arc 1 y 2
                                        lineStartPoint = new Point3D(lineX1, lineY1, 0.0); // Arc 1 start point
                                        lineEndPoint = new Point3D(lineX2, lineY2, 0.0); // Arc 1 end point
                                        if (entity is ArcGeometry nextArc && (entity.Color != 10) && (entity.Color != 11) && step == 3)
                                        {
                                            nextArcX1 = nextArc.EndPoint1.x;
                                            nextArcX2 = nextArc.EndPoint2.x;
                                            nextArcY1 = nextArc.EndPoint1.y;
                                            nextArcY2 = nextArc.EndPoint2.y;
                                            nextArcStartPoint = new Point3D(nextArcX1, nextArcY1, 0.0); // Arc 2 start point
                                            nextArcEndPoint = new Point3D(nextArcX2, nextArcY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(lineEndPoint, nextArcStartPoint);
                                            var entityEndDistance = VectorManager.Distance(lineEndPoint, nextArcEndPoint);
                                            if (entityStartDistance <= tolerance && step == 3)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 3;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 2;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 3)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 5;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 4;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                        if (entity is LineGeometry nextLine && (entity.Color != 10) && (entity.Color != 11) && step == 3)
                                        {
                                            nextLineX1 = nextLine.EndPoint1.x;
                                            nextLineX2 = nextLine.EndPoint2.x;
                                            nextLineY1 = nextLine.EndPoint1.y;
                                            nextLineY2 = nextLine.EndPoint2.y;
                                            nextLineStartPoint = new Point3D(nextLineX1, nextLineY1, 0.0); // Arc 2 start point
                                            nextLineEndPoint = new Point3D(nextLineX2, nextLineY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(lineEndPoint, nextLineStartPoint);
                                            var entityEndDistance = VectorManager.Distance(lineEndPoint, nextLineEndPoint);
                                            if (entityStartDistance <= tolerance && step == 3)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 3;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 2;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 3)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 5;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 4;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    } // Gets Geo that connects first entity end point to second entity start point (offset 1)
                    if (step == 4)
                    {
                        foreach (var entity in chainEntity)
                        {
                            if (entity.Color != 10 || entity.Color != 11)
                            {
                                if (firstEntity == 1 && step == 4)
                                {
                                    var thisEntity = ArcGeometry.RetrieveEntity(templist10[0]);
                                    if (thisEntity is ArcGeometry arc && step == 4)
                                    {
                                        arcX1 = arc.EndPoint1.x; // Arc 1 x 1
                                        arcX2 = arc.EndPoint2.x; // Arc 1 x 2
                                        arcY1 = arc.EndPoint1.y; // Arc 1 y 1
                                        arcY2 = arc.EndPoint2.y; // Arc 1 y 2
                                        arcEndPoint = new Point3D(arcX1, arcY1, 0.0); // Arc 1 start point
                                        arcStartPoint = new Point3D(arcX2, arcY2, 0.0); // Arc 1 end point
                                        if (entity is ArcGeometry nextArc && (entity.Color != 10) && (entity.Color != 11) && step == 4)
                                        {
                                            nextArcX1 = nextArc.EndPoint1.x;
                                            nextArcX2 = nextArc.EndPoint2.x;
                                            nextArcY1 = nextArc.EndPoint1.y;
                                            nextArcY2 = nextArc.EndPoint2.y;
                                            nextArcStartPoint = new Point3D(nextArcX1, nextArcY1, 0.0); // Arc 2 start point
                                            nextArcEndPoint = new Point3D(nextArcX2, nextArcY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(arcEndPoint, nextArcStartPoint);
                                            var entityEndDistance = VectorManager.Distance(arcEndPoint, nextArcEndPoint);
                                            if (entityStartDistance <= tolerance && step == 4)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 2;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 3;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 4)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 4;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 5;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                        if (entity is LineGeometry nextLine && (entity.Color != 10) && (entity.Color != 11) && step == 4)
                                        {
                                            nextLineX1 = nextLine.EndPoint1.x;
                                            nextLineX2 = nextLine.EndPoint2.x;
                                            nextLineY1 = nextLine.EndPoint1.y;
                                            nextLineY2 = nextLine.EndPoint2.y;
                                            nextLineStartPoint = new Point3D(nextLineX1, nextLineY1, 0.0); // Arc 2 start point
                                            nextLineEndPoint = new Point3D(nextLineX2, nextLineY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(arcEndPoint, nextLineStartPoint);
                                            var entityEndDistance = VectorManager.Distance(arcEndPoint, nextLineEndPoint);
                                            if (entityStartDistance <= tolerance && step == 4)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 2;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 3;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 4)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 4;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 5;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                    }
                                }
                                if (firstEntity == 2 && step == 4)
                                {
                                    var thisEntity = LineGeometry.RetrieveEntity(templist10[0]);
                                    if (thisEntity is LineGeometry line && step == 4)
                                    {
                                        lineX1 = line.EndPoint1.x; // Arc 1 x 1
                                        lineX2 = line.EndPoint2.x; // Arc 1 x 2
                                        lineY1 = line.EndPoint1.y; // Arc 1 y 1
                                        lineY2 = line.EndPoint2.y; // Arc 1 y 2
                                        lineEndPoint = new Point3D(lineX1, lineY1, 0.0); // Arc 1 start point
                                        lineStartPoint = new Point3D(lineX2, lineY2, 0.0); // Arc 1 end point
                                        if (entity is ArcGeometry nextArc && (entity.Color != 10) && (entity.Color != 11) && step == 4)
                                        {
                                            nextArcX1 = nextArc.EndPoint1.x;
                                            nextArcX2 = nextArc.EndPoint2.x;
                                            nextArcY1 = nextArc.EndPoint1.y;
                                            nextArcY2 = nextArc.EndPoint2.y;
                                            nextArcStartPoint = new Point3D(nextArcX1, nextArcY1, 0.0); // Arc 2 start point
                                            nextArcEndPoint = new Point3D(nextArcX2, nextArcY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(lineEndPoint, nextArcStartPoint);
                                            var entityEndDistance = VectorManager.Distance(lineEndPoint, nextArcEndPoint);
                                            if (entityStartDistance <= tolerance && step == 4)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 2;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 3;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 4)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 4;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 5;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                        if (entity is LineGeometry nextLine && (entity.Color != 10) && (entity.Color != 11) && step == 4)
                                        {
                                            nextLineX1 = nextLine.EndPoint1.x;
                                            nextLineX2 = nextLine.EndPoint2.x;
                                            nextLineY1 = nextLine.EndPoint1.y;
                                            nextLineY2 = nextLine.EndPoint2.y;
                                            nextLineStartPoint = new Point3D(nextLineX1, nextLineY1, 0.0); // Arc 2 start point
                                            nextLineEndPoint = new Point3D(nextLineX2, nextLineY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(lineEndPoint, nextLineStartPoint);
                                            var entityEndDistance = VectorManager.Distance(lineEndPoint, nextLineEndPoint);
                                            if (entityStartDistance <= tolerance && step == 4)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 2;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 3;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 4)
                                            {
                                                entity.Color = 10;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 4;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 5;

                                                        }

                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    } // Gets Geo that connects first entity start point to second entity start point (offset 2)
                    if (step == 5)
                    {
                        foreach (var entity in chainEntity)
                        {
                            if (entity.Color != 10 || entity.Color != 11)
                            {
                                if (firstEntity == 1 && step == 5)
                                {
                                    var thisEntity = ArcGeometry.RetrieveEntity(templist10[0]);
                                    if (thisEntity is ArcGeometry arc && step == 5)
                                    {
                                        arcX1 = arc.EndPoint1.x; // Arc 1 x 1
                                        arcX2 = arc.EndPoint2.x; // Arc 1 x 2
                                        arcY1 = arc.EndPoint1.y; // Arc 1 y 1
                                        arcY2 = arc.EndPoint2.y; // Arc 1 y 2
                                        arcEndPoint = new Point3D(arcX1, arcY1, 0.0); // Arc 1 start point
                                        arcStartPoint = new Point3D(arcX2, arcY2, 0.0); // Arc 1 end point
                                        if (entity is ArcGeometry nextArc && (entity.Color != 10) && (entity.Color != 11) && step == 5)
                                        {
                                            nextArcX1 = nextArc.EndPoint1.x;
                                            nextArcX2 = nextArc.EndPoint2.x;
                                            nextArcY1 = nextArc.EndPoint1.y;
                                            nextArcY2 = nextArc.EndPoint2.y;
                                            nextArcStartPoint = new Point3D(nextArcX1, nextArcY1, 0.0); // Arc 2 start point
                                            nextArcEndPoint = new Point3D(nextArcX2, nextArcY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(arcEndPoint, nextArcStartPoint);
                                            var entityEndDistance = VectorManager.Distance(arcEndPoint, nextArcEndPoint);
                                            if (entityStartDistance <= tolerance && step == 5)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 3;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 2;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 5)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 5;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 4;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                        if (entity is LineGeometry nextLine && (entity.Color != 10) && (entity.Color != 11) && step == 5)
                                        {
                                            nextLineX1 = nextLine.EndPoint1.x;
                                            nextLineX2 = nextLine.EndPoint2.x;
                                            nextLineY1 = nextLine.EndPoint1.y;
                                            nextLineY2 = nextLine.EndPoint2.y;
                                            nextLineStartPoint = new Point3D(nextLineX1, nextLineY1, 0.0); // Arc 2 start point
                                            nextLineEndPoint = new Point3D(nextLineX2, nextLineY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(arcEndPoint, nextLineStartPoint);
                                            var entityEndDistance = VectorManager.Distance(arcEndPoint, nextLineEndPoint);
                                            if (entityStartDistance <= tolerance && step == 5)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 3;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 2;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 5)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 5;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 4;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                    }
                                }
                                if (firstEntity == 2 && step == 5)
                                {
                                    var thisEntity = LineGeometry.RetrieveEntity(templist10[0]);
                                    if (thisEntity is LineGeometry line && step == 5)
                                    {
                                        lineX1 = line.EndPoint1.x; // Arc 1 x 1
                                        lineX2 = line.EndPoint2.x; // Arc 1 x 2
                                        lineY1 = line.EndPoint1.y; // Arc 1 y 1
                                        lineY2 = line.EndPoint2.y; // Arc 1 y 2
                                        lineEndPoint = new Point3D(lineX1, lineY1, 0.0); // Arc 1 start point
                                        lineStartPoint = new Point3D(lineX2, lineY2, 0.0); // Arc 1 end point
                                        if (entity is ArcGeometry nextArc && (entity.Color != 10) && (entity.Color != 11) && step == 5)
                                        {
                                            nextArcX1 = nextArc.EndPoint1.x;
                                            nextArcX2 = nextArc.EndPoint2.x;
                                            nextArcY1 = nextArc.EndPoint1.y;
                                            nextArcY2 = nextArc.EndPoint2.y;
                                            nextArcStartPoint = new Point3D(nextArcX1, nextArcY1, 0.0); // Arc 2 start point
                                            nextArcEndPoint = new Point3D(nextArcX2, nextArcY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(lineEndPoint, nextArcStartPoint);
                                            var entityEndDistance = VectorManager.Distance(lineEndPoint, nextArcEndPoint);
                                            if (entityStartDistance <= tolerance && step == 5)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 3;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 2;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 5)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 5;
                                                firstEntity = 1;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextArcStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 4;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                        if (entity is LineGeometry nextLine && (entity.Color != 10) && (entity.Color != 11) && step == 5)
                                        {
                                            nextLineX1 = nextLine.EndPoint1.x;
                                            nextLineX2 = nextLine.EndPoint2.x;
                                            nextLineY1 = nextLine.EndPoint1.y;
                                            nextLineY2 = nextLine.EndPoint2.y;
                                            nextLineStartPoint = new Point3D(nextLineX1, nextLineY1, 0.0); // Arc 2 start point
                                            nextLineEndPoint = new Point3D(nextLineX2, nextLineY2, 0.0); // Arc 2 end point
                                            var entityStartDistance = VectorManager.Distance(lineEndPoint, nextLineStartPoint);
                                            var entityEndDistance = VectorManager.Distance(lineEndPoint, nextLineEndPoint);
                                            if (entityStartDistance <= tolerance && step == 5)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 3;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineEndPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 2;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                            if (entityEndDistance <= tolerance && step == 5)
                                            {
                                                entity.Color = 11;
                                                entity.Commit();
                                                templist10.Clear();
                                                templist10.Add(entity.GetEntityID());
                                                step = 5;
                                                firstEntity = 2;
                                                foreach (var v in tempList9)
                                                {
                                                    var w = PointGeometry.RetrieveEntity(v);// Gets point based on GeoID
                                                    if (w is PointGeometry point)
                                                    {
                                                        var ux = point.Data.x;// X location of point
                                                        var uy = point.Data.y;// Y location of point
                                                        Point3D uPoint = new Point3D(ux, uy, 0.0);// Saves point location
                                                        var entityPointDistance = VectorManager.Distance(uPoint, nextLineStartPoint);
                                                        if (entityPointDistance <= tolerance)
                                                        {
                                                            step = 4;
                                                        }
                                                    }
                                                }
                                                goto stepStart;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    } // Gets Geo that connects first entity start point to second entity start point (offset 1)

                }
                selectedChains = null;
            } // Color cut sides of each section
            void movePoints()
            {
                SelectionManager.SelectGeometryByMask(Mastercam.IO.Types.QuickMaskType.Points);
                var selectedGeometry = SearchManager.GetSelectedGeometry();
                foreach (var point in selectedGeometry)
                {
                    point.Level = 90;
                    point.Selected = false;
                    point.Commit();
                }
                selectedGeometry = null;

            } // Moves intersection points to different level
            void seperateGeometry()
            {
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(75);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown)
                {
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(75, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);
                SelectionManager.SelectAllGeometry();
                var selectedGeometry = SearchManager.GetSelectedGeometry();
                foreach (var geometry in selectedGeometry)
                {
                    if (geometry.Color == 10)
                    {
                        geometry.Level = 80;
                        geometry.Selected = false;
                        geometry.Commit();
                    }
                    if (geometry.Color == 11)
                    {
                        geometry.Level = 81;
                        geometry.Selected = false;
                        geometry.Commit();
                    }
                }
                //invalidate the list of geometry
                selectedGeometry = null;
                //ask to reclaim the memory from the list
                System.GC.Collect();
                //repaint the scren
                Mastercam.IO.GraphicsManager.Repaint(true);//force rebuild

                SelectionManager.UnselectAllGeometry();
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);

            } // Moves cut side and non-cut side geometry to  different levels
            void offsetCutchain80()
            {

                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(80);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown)
                {
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(80, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);
                int createdUpperLevel = 500;
                int createdLowerLevel = 501;
                LevelsManager.SetLevelName(500, "Upper Created Cut Geo");
                LevelsManager.SetLevelName(501, "Lower Created Cut Geo");

                var commonParams = new BoundingBoxCommonParams { CreateLinesArcs = true };
                var rectangularParams = new BoundingBoxRectangularParams { ExpandXMinus = 2.0, ExpandXPlus = 2.0, ExpandYMinus = 2.0, ExpandYPlus = 2.0 };
                var boundingBox = Mastercam.GeometryUtility.GeometryCreationManager.RectangularBoundingBox(commonParams, rectangularParams);
                var boundingBoxChain = ChainManager.ChainGeometry(boundingBox);

                GraphicsManager.FitScreen();
                void SendLeftClick(int posX, int posY)
                {
                    Cursor.Position = new Point(posX, posY);
                    mouse_event(MouseEventLeftDown, 0, 0, 0, new System.IntPtr());
                    Thread.Sleep(100);
                    mouse_event(MouseEventLeftUp, 0, 0, 0, new System.IntPtr());
                }
                new System.Threading.Timer((x) => { GraphicsManager.SetFocusToGraphicsWindow(); }, null, 1000, Timeout.Infinite);
                new System.Threading.Timer((x) => { SendKeys.SendWait("{A}"); }, null, 2000, Timeout.Infinite);
                new System.Threading.Timer((x) => { SendLeftClick(1200, 600); }, null, 3000, Timeout.Infinite);
                new System.Threading.Timer((x) => { SendKeys.SendWait("{ENTER}"); }, null, 4000, Timeout.Infinite);

                var selectedCutChain = ChainManager.GetMultipleChains("");
                foreach (var chain in selectedCutChain)
                {
                    foreach (var t in boundingBoxChain)
                    {
                        if (chain.Area != t.Area)
                        {
                            chain.Direction = ChainDirectionType.Clockwise;
                            var lowerChainLarge = chain.OffsetChain2D(OffsetSideType.Right, .0225, OffsetRollCornerType.None, .5, false, .005, false);
                            var lowerChainSmall = chain.OffsetChain2D(OffsetSideType.Left, .0025, OffsetRollCornerType.None, .5, false, .005, false);
                            var cutResultGeometry = SearchManager.GetResultGeometry();
                            foreach (var entity in cutResultGeometry)
                            {
                                entity.Color = 11;
                                entity.Level = createdLowerLevel;
                                entity.Selected = false;
                                entity.Commit();
                            }
                            GraphicsManager.ClearColors(new GroupSelectionMask(true));
                            var upperChainLarge = chain.OffsetChain2D(OffsetSideType.Right, .0025, OffsetRollCornerType.None, .5, false, .005, false);
                            var upperChainSmall = chain.OffsetChain2D(OffsetSideType.Left, .0385, OffsetRollCornerType.None, .5, false, .005, false);
                            var cutResultGeometryNew = SearchManager.GetResultGeometry();
                            foreach (var entity in cutResultGeometryNew)
                            {
                                entity.Color = 10;
                                entity.Level = createdUpperLevel;
                                entity.Selected = false;
                                entity.Commit();
                            }
                            GraphicsManager.ClearColors(new GroupSelectionMask(true));
                        }
                    }

                }
                foreach (var entity in boundingBox)
                {
                    entity.Retrieve();
                    entity.Delete();
                }
            } // Offsets cut side geometry
            void offsetCutchain81()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(81);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown)
                {
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(81, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);//Required
                int createdUpperLevel = 500;
                int createdLowerLevel = 501;
                LevelsManager.SetLevelName(500, "Upper Created Cut Geo");
                LevelsManager.SetLevelName(501, "Lower Created Cut Geo");

                var commonParams = new BoundingBoxCommonParams { CreateLinesArcs = true };
                var rectangularParams = new BoundingBoxRectangularParams { ExpandXMinus = 2.0, ExpandXPlus = 2.0, ExpandYMinus = 2.0, ExpandYPlus = 2.0 };
                var boundingBox = Mastercam.GeometryUtility.GeometryCreationManager.RectangularBoundingBox(commonParams, rectangularParams);
                var boundingBoxChain = ChainManager.ChainGeometry(boundingBox);

                GraphicsManager.FitScreen();
                void SendLeftClick(int posX, int posY)
                {
                    Cursor.Position = new Point(posX, posY);
                    mouse_event(MouseEventLeftDown, 0, 0, 0, new System.IntPtr());
                    Thread.Sleep(100);
                    mouse_event(MouseEventLeftUp, 0, 0, 0, new System.IntPtr());
                }
                new System.Threading.Timer((x) => { GraphicsManager.SetFocusToGraphicsWindow(); }, null, 1000, Timeout.Infinite);
                new System.Threading.Timer((x) => { SendKeys.SendWait("{A}"); }, null, 2000, Timeout.Infinite);
                new System.Threading.Timer((x) => { SendLeftClick(1200, 600); }, null, 3000, Timeout.Infinite);
                new System.Threading.Timer((x) => { SendKeys.SendWait("{ENTER}"); }, null, 4000, Timeout.Infinite);

                var selectedCutChain = ChainManager.GetMultipleChains("");
                var chainDirection = ChainDirectionType.Clockwise;
                foreach (var chain in selectedCutChain)
                {
                    foreach (var t in boundingBoxChain)
                    {
                        if (chain.Area != t.Area)
                        {
                            chain.Direction = chainDirection;
                            var lowerChainLarge = chain.OffsetChain2D(OffsetSideType.Left, .0225, OffsetRollCornerType.None, .5, false, .005, false);
                            var lowerChainSmall = chain.OffsetChain2D(OffsetSideType.Right, .0025, OffsetRollCornerType.None, .5, false, .005, false);
                            var cutResultGeometry = SearchManager.GetResultGeometry();
                            foreach (var entity in cutResultGeometry)
                            {
                                entity.Color = 11;
                                entity.Level = createdLowerLevel;
                                entity.Selected = false;
                                entity.Commit();
                            }
                            GraphicsManager.ClearColors(new GroupSelectionMask(true));

                            var upperChainLarge = chain.OffsetChain2D(OffsetSideType.Left, .0025, OffsetRollCornerType.None, .5, false, .005, false);
                            var upperChainSmall = chain.OffsetChain2D(OffsetSideType.Right, .0385, OffsetRollCornerType.None, .5, false, .005, false);
                            var cutResultGeometryNew = SearchManager.GetResultGeometry();
                            foreach (var entity in cutResultGeometryNew)
                            {
                                entity.Color = 10;
                                entity.Level = createdUpperLevel;
                                entity.Selected = false;
                                entity.Commit();
                            }
                            GraphicsManager.ClearColors(new GroupSelectionMask(true));
                        }
                    }
                }
                foreach (var entity in boundingBox)
                {
                    entity.Retrieve();
                    entity.Delete();
                }
            } // offsets non-cut side geometry
            void shortenChains500()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(500);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown)
                {
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(500, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);//required
                var chainDetails = new Mastercam.Database.Interop.ChainDetails();// Preps the ChainDetails plugin
                var selectedChains = ChainManager.ChainAll(500);
                var chainDirection = ChainDirectionType.CounterClockwise;// Going to be used to make sure all chains go the same direction
                ChainManager.StartChainAtLongest(selectedChains);
                foreach (var chain in selectedChains)
                {
                    chain.Direction = chainDirection;
                    var chainData = chainDetails.GetData(chain);
                    var firstEntity = chainData.FirstEntity.GetEntityID();
                    var lastEntity = chainData.LastEntity.GetEntityID();
                    var firstIsFlipped = chainData.FirstEntityIsFlipped;
                    var lastIsFlipped = chainData.LastEntityIsFlipped;
                    if (firstIsFlipped == false)
                    {
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(firstEntity);
                        if (entity is LineGeometry line)
                        {
                            Point3D lineStartPoint = (line.Data.Point1);
                            Point3D lineEndPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineEndPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                            entity.Commit();
                        }
                    }
                    if (firstIsFlipped == true)
                    {
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(firstEntity);
                        if (entity is LineGeometry line)
                        {
                            Point3D lineEndPoint = (line.Data.Point1);
                            Point3D lineStartPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineEndPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                            entity.Commit();
                        }
                    }
                    if (lastIsFlipped == false)
                    {
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(lastEntity);
                        if (entity is LineGeometry line)
                        {
                            Point3D lineStartPoint = (line.Data.Point1);
                            Point3D lineEndPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineStartPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                            entity.Commit();
                        }
                    }
                    if (lastIsFlipped == true)
                    {
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(lastEntity);
                        if (entity is LineGeometry line)
                        {
                            Point3D lineEndPoint = (line.Data.Point1);
                            Point3D lineStartPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineStartPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                            entity.Commit();
                        }
                    }
                }
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
            } // Shortens chains on level 500
            void shortenChains501()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(501);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown)
                {
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(501, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);//required
                var chainDetails = new Mastercam.Database.Interop.ChainDetails();// Preps the ChainDetails plugin
                var selectedChains = ChainManager.ChainAll(501);
                var chainDirection = ChainDirectionType.CounterClockwise;// Going to be used to make sure all chains go the same direction
                ChainManager.StartChainAtLongest(selectedChains);
                foreach (var chain in selectedChains)
                {
                    chain.Direction = chainDirection;
                    var chainData = chainDetails.GetData(chain);
                    var firstEntity = chainData.FirstEntity.GetEntityID();
                    var lastEntity = chainData.LastEntity.GetEntityID();
                    var firstIsFlipped = chainData.FirstEntityIsFlipped;
                    var lastIsFlipped = chainData.LastEntityIsFlipped;
                    if (firstIsFlipped == false)
                    {
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(firstEntity);
                        if (entity is LineGeometry line)
                        {
                            Point3D lineStartPoint = (line.Data.Point1);
                            Point3D lineEndPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineEndPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                            entity.Commit();
                        }
                    }
                    if (firstIsFlipped == true)
                    {
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(firstEntity);
                        if (entity is LineGeometry line)
                        {
                            Point3D lineEndPoint = (line.Data.Point1);
                            Point3D lineStartPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineEndPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                            entity.Commit();
                        }
                    }
                    if (lastIsFlipped == false)
                    {
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(lastEntity);
                        if (entity is LineGeometry line)
                        {
                            Point3D lineStartPoint = (line.Data.Point1);
                            Point3D lineEndPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineStartPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                            entity.Commit();
                        }
                    }
                    if (lastIsFlipped == true)
                    {
                        var entity = Mastercam.Database.Geometry.RetrieveEntity(lastEntity);
                        if (entity is LineGeometry line)
                        {
                            Point3D lineEndPoint = (line.Data.Point1);
                            Point3D lineStartPoint = (line.Data.Point2);
                            var lineLength = VectorManager.Distance(lineStartPoint, lineEndPoint);
                            var scale = ((lineLength - 0.010) / lineLength);
                            entity.Scale(lineStartPoint, scale);
                            entity.Commit();
                        }
                        if (entity is ArcGeometry arc)
                        {
                            var arcStartPoint = (arc.Data.StartAngleDegrees);
                            var arcEndPoint = (arc.Data.EndAngleDegrees);
                            var arcRad = arc.Data.Radius;
                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                            var newArcLength = (arcLength - 0.010);
                            var circumference = 2 * Math.PI * arcRad;
                            var arcMeasure = ((newArcLength / circumference) * 360);
                            arc.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                            entity.Commit();
                        }
                    }
                }
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
            } // Shortens chains on level 501
            void findArcChainEnds500()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(500);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown)
                {
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(500, true);
                LevelsManager.RefreshLevelsManager();
                //GraphicsManager.Repaint(true);
                var chainDetails = new Mastercam.Database.Interop.ChainDetails();// Preps the ChainDetails plugin
                var selectedChains = ChainManager.ChainAll(500);
                var chainDirection = ChainDirectionType.CounterClockwise;// Going to be used to make sure all chains go the same direction
                ChainManager.StartChainAtLongest(selectedChains);
                foreach (var chain in selectedChains)
                {
                    chain.Direction = chainDirection;
                    var chainData = chainDetails.GetData(chain);
                    var firstEntity = chainData.FirstEntity.GetEntityID();
                    var lastEntity = chainData.LastEntity.GetEntityID();
                    var firstIsFlipped = chainData.FirstEntityIsFlipped;
                    var lastIsFlipped = chainData.LastEntityIsFlipped;
                    if (Geometry.RetrieveEntity(firstEntity) is ArcGeometry startArc)
                    {
                        startArc.Color = 70;
                        startArc.Commit();
                        templist500.Add(firstEntity);
                        anotherTemplist500.Add(firstEntity);
                    }
                    if (Geometry.RetrieveEntity(lastEntity) is ArcGeometry endArc)
                    {
                        endArc.Color = 70;
                        endArc.Commit();
                        templist500.Add(lastEntity);
                        anotherTemplist500.Add(lastEntity);
                    }
                }
                var tempListArc = new List<int>();
                var tempListRad = new List<double>();
                var rad1 = 0;
                var rad2 = 0;
                var rad3 = 0;
                var rad4 = 0;
                foreach (var arc in templist500)
                {
                    if (Geometry.RetrieveEntity(arc) is ArcGeometry firstArc && firstArc.Color != 60)
                    {
                        tempListArc.Add(arc);
                        tempListRad.Add(firstArc.Data.Radius);
                        var firstArcCP = firstArc.Data.CenterPoint;
                        foreach (var anotherArc in templist500)
                        {
                            if (Geometry.RetrieveEntity(anotherArc) is ArcGeometry nextArc && anotherArc != arc && nextArc.Color != 60)
                            {
                                var nextArcCP = nextArc.Data.CenterPoint;
                                if (VectorManager.Distance(firstArcCP, nextArcCP) < 0.001)
                                {
                                    tempListRad.Add(nextArc.Data.Radius);
                                    tempListArc.Add(anotherArc);
                                    if (tempListArc.Count == 4)
                                    {
                                        foreach (var thisArc in tempListArc)
                                        {
                                            var entity = Geometry.RetrieveEntity(thisArc);
                                            entity.Color = 60;
                                            entity.Commit();
                                            tempListRad.Sort();
                                        }
                                        foreach (var i in tempListArc)
                                        {
                                            var thisEntity = Geometry.RetrieveEntity(i);
                                            if (thisEntity is ArcGeometry rad)
                                            {
                                                if (rad.Data.Radius == tempListRad[0])
                                                {
                                                    rad1 = (thisEntity.GetEntityID());
                                                }
                                                if (rad.Data.Radius == tempListRad[1])
                                                {
                                                    rad2 = (thisEntity.GetEntityID());
                                                }
                                                if (rad.Data.Radius == tempListRad[2])
                                                {
                                                    rad3 = (thisEntity.GetEntityID());
                                                }
                                                if (rad.Data.Radius == tempListRad[3])
                                                {
                                                    rad4 = (thisEntity.GetEntityID());
                                                }
                                            }
                                        }
                                        var newRad1 = Geometry.RetrieveEntity(rad1);
                                        var newRad2 = Geometry.RetrieveEntity(rad2);
                                        var newRad3 = Geometry.RetrieveEntity(rad3);
                                        var newRad4 = Geometry.RetrieveEntity(rad4);
                                        var arc1End = new Point3D();
                                        var arc2End = new Point3D();
                                        var arc3End = new Point3D();
                                        var arc4End = new Point3D();
                                        newRad1.Commit();
                                        newRad2.Commit();
                                        newRad3.Commit();
                                        newRad4.Commit();

                                        if (newRad2 is ArcGeometry arc2)
                                        {
                                            var arcStartPoint = (arc2.Data.StartAngleDegrees);
                                            var arcEndPoint = (arc2.Data.EndAngleDegrees);
                                            var arcRad = arc2.Data.Radius;
                                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                                            var newArcLength = (arcLength + 0.020);
                                            var circumference = 2 * Math.PI * arcRad;
                                            var arcMeasure = ((newArcLength / circumference) * 360);
                                            if ((arc2.Data.StartAngleDegrees + 360) > (arc2.Data.EndAngleDegrees + 360) && (arc2.Data.StartAngleDegrees >= 90) && (arc2.Data.StartAngleDegrees <= 260))
                                            {
                                                arc2.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                                                newRad2.Commit();
                                                arc2End = new Point3D(arc2.EndPoint2.x, arc2.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc2End);
                                            }
                                            else
                                            {
                                                arc2.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                                                newRad2.Commit();
                                                arc2End = new Point3D(arc2.EndPoint1.x, arc2.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc2End);
                                            }
                                        }
                                        if (newRad3 is ArcGeometry arc3)
                                        {
                                            var arcStartPoint = (arc3.Data.StartAngleDegrees);
                                            var arcEndPoint = (arc3.Data.EndAngleDegrees);
                                            var arcRad = arc3.Data.Radius;
                                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                                            var newArcLength = (arcLength + 0.020);
                                            var circumference = 2 * Math.PI * arcRad;
                                            var arcMeasure = ((newArcLength / circumference) * 360);
                                            if ((arc3.Data.StartAngleDegrees + 360) > (arc3.Data.EndAngleDegrees + 360) && (arc3.Data.StartAngleDegrees >= 90) && (arc3.Data.StartAngleDegrees <= 260))
                                            {
                                                arc3.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                                                newRad3.Commit();
                                                arc3End = new Point3D(arc3.EndPoint1.x, arc3.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc3End);
                                            }
                                            else
                                            {
                                                arc3.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                                                newRad3.Commit();
                                                arc3End = new Point3D(arc3.EndPoint2.x, arc3.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc3End);
                                            }
                                        }
                                        if (newRad1 is ArcGeometry arc1)
                                        {
                                            if ((arc1.Data.StartAngleDegrees + 360) > (arc1.Data.EndAngleDegrees + 360) && (arc1.Data.StartAngleDegrees >= 90) && (arc1.Data.StartAngleDegrees <= 260))
                                            {
                                                arc1End = new Point3D(arc1.EndPoint1.x, arc1.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc1End);
                                            }
                                            else
                                            {
                                                arc1End = new Point3D(arc1.EndPoint2.x, arc1.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc1End);
                                            }
                                        }
                                        if (newRad4 is ArcGeometry arc4)
                                        {
                                            if ((arc4.Data.StartAngleDegrees + 360) > (arc4.Data.EndAngleDegrees + 360) && (arc4.Data.StartAngleDegrees >= 90) && (arc4.Data.StartAngleDegrees <= 260))
                                            {
                                                arc4End = new Point3D(arc4.EndPoint2.x, arc4.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc4End);
                                            }
                                            else
                                            {
                                                arc4End = new Point3D(arc4.EndPoint1.x, arc4.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc4End);
                                            }
                                        }

                                        var line1 = new LineGeometry(arc1End, arc2End);
                                        var line2 = new LineGeometry(arc3End, arc4End);
                                        line1.Color = 10;
                                        line2.Color = 10;
                                        line1.Commit();
                                        line2.Commit();
                                        tempListArc.Clear();
                                        tempListRad.Clear();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var chain in ChainManager.ChainAll(500))
                {
                    var chainGeo = ChainManager.GetGeometryInChain(chain);
                    foreach (var entity in chainGeo)
                    {
                        entity.Color = 70;
                        entity.Selected = false;
                        entity.Commit();
                    }
                }
                SelectionManager.UnselectAllGeometry();
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                GraphicsManager.Repaint();
            }
            void findArcChainEnds501()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(501);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown)
                {
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.SetLevelVisible(501, true);
                LevelsManager.RefreshLevelsManager();
                //GraphicsManager.Repaint(true);
                var chainDetails = new Mastercam.Database.Interop.ChainDetails();// Preps the ChainDetails plugin
                var selectedChains = ChainManager.ChainAll(501);
                var chainDirection = ChainDirectionType.CounterClockwise;// Going to be used to make sure all chains go the same direction
                ChainManager.StartChainAtLongest(selectedChains);
                foreach (var chain in selectedChains)
                {
                    chain.Direction = chainDirection;
                    var chainData = chainDetails.GetData(chain);
                    var firstEntity = chainData.FirstEntity.GetEntityID();
                    var lastEntity = chainData.LastEntity.GetEntityID();
                    var firstIsFlipped = chainData.FirstEntityIsFlipped;
                    var lastIsFlipped = chainData.LastEntityIsFlipped;
                    if (Geometry.RetrieveEntity(firstEntity) is ArcGeometry startArc)
                    {
                        startArc.Color = 70;
                        startArc.Commit();
                        templist501.Add(firstEntity);
                        anotherTemplist501.Add(firstEntity);
                    }
                    if (Geometry.RetrieveEntity(lastEntity) is ArcGeometry endArc)
                    {
                        endArc.Color = 70;
                        endArc.Commit();
                        templist501.Add(lastEntity);
                        anotherTemplist501.Add(lastEntity);
                    }
                }
                var tempListArc = new List<int>();
                var tempListRad = new List<double>();
                var rad1 = 0;
                var rad2 = 0;
                var rad3 = 0;
                var rad4 = 0;
                foreach (var arc in templist501)
                {
                    if (Geometry.RetrieveEntity(arc) is ArcGeometry firstArc && firstArc.Color != 60)
                    {
                        tempListArc.Add(arc);
                        tempListRad.Add(firstArc.Data.Radius);
                        var firstArcCP = firstArc.Data.CenterPoint;
                        foreach (var anotherArc in templist501)
                        {
                            if (Geometry.RetrieveEntity(anotherArc) is ArcGeometry nextArc && anotherArc != arc && nextArc.Color != 60)
                            {
                                var nextArcCP = nextArc.Data.CenterPoint;
                                if (VectorManager.Distance(firstArcCP, nextArcCP) < 0.001)
                                {
                                    tempListRad.Add(nextArc.Data.Radius);
                                    tempListArc.Add(anotherArc);
                                    if (tempListArc.Count == 4)
                                    {
                                        foreach (var thisArc in tempListArc)
                                        {
                                            var entity = Geometry.RetrieveEntity(thisArc);
                                            entity.Color = 60;
                                            entity.Commit();
                                            tempListRad.Sort();
                                        }
                                        foreach (var i in tempListArc)
                                        {
                                            var thisEntity = Geometry.RetrieveEntity(i);
                                            if (thisEntity is ArcGeometry rad)
                                            {
                                                if (rad.Data.Radius == tempListRad[0])
                                                {
                                                    rad1 = (thisEntity.GetEntityID());
                                                }
                                                if (rad.Data.Radius == tempListRad[1])
                                                {
                                                    rad2 = (thisEntity.GetEntityID());
                                                }
                                                if (rad.Data.Radius == tempListRad[2])
                                                {
                                                    rad3 = (thisEntity.GetEntityID());
                                                }
                                                if (rad.Data.Radius == tempListRad[3])
                                                {
                                                    rad4 = (thisEntity.GetEntityID());
                                                }
                                            }
                                        }
                                        var newRad1 = Geometry.RetrieveEntity(rad1);
                                        var newRad2 = Geometry.RetrieveEntity(rad2);
                                        var newRad3 = Geometry.RetrieveEntity(rad3);
                                        var newRad4 = Geometry.RetrieveEntity(rad4);
                                        var arc1End = new Point3D();
                                        var arc2End = new Point3D();
                                        var arc3End = new Point3D();
                                        var arc4End = new Point3D();
                                        newRad1.Commit();
                                        newRad2.Commit();
                                        newRad3.Commit();
                                        newRad4.Commit();

                                        if (newRad2 is ArcGeometry arc2)
                                        {
                                            var arcStartPoint = (arc2.Data.StartAngleDegrees);
                                            var arcEndPoint = (arc2.Data.EndAngleDegrees);
                                            var arcRad = arc2.Data.Radius;
                                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                                            var newArcLength = (arcLength + 0.020);
                                            var circumference = 2 * Math.PI * arcRad;
                                            var arcMeasure = ((newArcLength / circumference) * 360);
                                            if ((arc2.Data.StartAngleDegrees + 360) > (arc2.Data.EndAngleDegrees + 360) && (arc2.Data.StartAngleDegrees >= 90) && (arc2.Data.StartAngleDegrees <= 260))
                                            {
                                                arc2.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                                                newRad2.Commit();
                                                arc2End = new Point3D(arc2.EndPoint1.x, arc2.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc2End);
                                            }
                                            else
                                            {
                                                arc2.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                                                newRad2.Commit();
                                                arc2End = new Point3D(arc2.EndPoint2.x, arc2.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc2End);
                                            }
                                        }
                                        if (newRad3 is ArcGeometry arc3)
                                        {
                                            var arcStartPoint = (arc3.Data.StartAngleDegrees);
                                            var arcEndPoint = (arc3.Data.EndAngleDegrees);
                                            var arcRad = arc3.Data.Radius;
                                            var arcLength = ((VectorManager.DegreesToRadians(arcStartPoint - arcEndPoint)) * arcRad);
                                            var newArcLength = (arcLength + 0.020);
                                            var circumference = 2 * Math.PI * arcRad;
                                            var arcMeasure = ((newArcLength / circumference) * 360);
                                            if ((arc3.Data.StartAngleDegrees + 360) > (arc3.Data.EndAngleDegrees + 360) && (arc3.Data.StartAngleDegrees >= 90) && (arc3.Data.StartAngleDegrees <= 260))
                                            {
                                                arc3.Data.EndAngleDegrees = (arcStartPoint - arcMeasure);
                                                newRad3.Commit();
                                                arc3End = new Point3D(arc3.EndPoint2.x, arc3.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc3End);
                                            }
                                            else
                                            {
                                                arc3.Data.StartAngleDegrees = (arcEndPoint + arcMeasure);
                                                newRad3.Commit();
                                                arc3End = new Point3D(arc3.EndPoint1.x, arc3.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc3End);
                                            }
                                        }
                                        if (newRad1 is ArcGeometry arc1)
                                        {
                                            if ((arc1.Data.StartAngleDegrees + 360) > (arc1.Data.EndAngleDegrees + 360) && (arc1.Data.StartAngleDegrees >= 90) && (arc1.Data.StartAngleDegrees <= 260))
                                            {
                                                arc1End = new Point3D(arc1.EndPoint2.x, arc1.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc1End);
                                            }
                                            else
                                            {
                                                arc1End = new Point3D(arc1.EndPoint1.x, arc1.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc1End);
                                            }
                                        }
                                        if (newRad4 is ArcGeometry arc4)
                                        {
                                            if ((arc4.Data.StartAngleDegrees + 360) > (arc4.Data.EndAngleDegrees + 360) && (arc4.Data.StartAngleDegrees >= 90) && (arc4.Data.StartAngleDegrees <= 260))
                                            {
                                                arc4End = new Point3D(arc4.EndPoint1.x, arc4.EndPoint1.y, 0.0);
                                                var point = new PointGeometry(arc4End);
                                            }
                                            else
                                            {
                                                arc4End = new Point3D(arc4.EndPoint2.x, arc4.EndPoint2.y, 0.0);
                                                var point = new PointGeometry(arc4End);
                                            }
                                        }

                                        var line1 = new LineGeometry(arc1End, arc2End);
                                        var line2 = new LineGeometry(arc3End, arc4End);
                                        line1.Color = 10;
                                        line2.Color = 10;
                                        line1.Commit();
                                        line2.Commit();
                                        tempListArc.Clear();
                                        tempListRad.Clear();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var chain in ChainManager.ChainAll(501))
                {
                    var chainGeo = ChainManager.GetGeometryInChain(chain);
                    foreach (var entity in chainGeo)
                    {
                        entity.Color = 70;
                        entity.Selected = false;
                        entity.Commit();
                    }
                }
                SelectionManager.UnselectAllGeometry();
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                GraphicsManager.Repaint();
            }
            void findLineChainEnds500()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(500);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown) { LevelsManager.SetLevelVisible(level, false); }
                LevelsManager.SetLevelVisible(500, true);
                LevelsManager.SetLevelVisible(50, true);
                GraphicsManager.Repaint(true);
                LevelsManager.RefreshLevelsManager();
                var geoMask = new GeometryMask(true, false, false, false, false, false, false, false);
                var selectionMask = new SelectionMask(true);
                var tempPoints = SearchManager.GetGeometry(geoMask, selectionMask, 50);
                foreach (var point in tempPoints)
                {
                    point.Selected = true;
                    point.Commit();
                }
                var selectedGeometry = SearchManager.GetSelectedGeometry();
                foreach (var point in selectedGeometry) { pointList.Add(point.GetEntityID()); }
                LevelsManager.SetLevelVisible(50, false);
                LevelsManager.RefreshLevelsManager();
                //GraphicsManager.Repaint(true);
                var chainDetails = new Mastercam.Database.Interop.ChainDetails();// Preps the ChainDetails plugin
                var selectedChains = ChainManager.ChainAll(500);
                var chainDirection = ChainDirectionType.CounterClockwise;// Going to be used to make sure all chains go the same direction
                ChainManager.StartChainAtLongest(selectedChains);

                foreach (var chain in selectedChains)
                {
                    chain.Direction = chainDirection;
                    var chainData = chainDetails.GetData(chain);
                    var firstEntity = chainData.FirstEntity.GetEntityID();
                    var lastEntity = chainData.LastEntity.GetEntityID();
                    var firstIsFlipped = chainData.FirstEntityIsFlipped;
                    var lastIsFlipped = chainData.LastEntityIsFlipped;
                    if (Geometry.RetrieveEntity(firstEntity) is LineGeometry startLine)
                    {
                        startLine.Color = 70;
                        startLine.Commit();
                        templist500.Add(firstEntity);
                    }
                    if (Geometry.RetrieveEntity(lastEntity) is LineGeometry endLine)
                    {
                        endLine.Color = 70;
                        endLine.Commit();
                        templist500.Add(lastEntity);
                    }
                }
                foreach (var line in templist500)
                {
                    var thisLine = Geometry.RetrieveEntity(line);
                    if (thisLine is LineGeometry noThisLine)
                    {
                        var pt1 = noThisLine.Data.Point1;
                        var pt2 = noThisLine.Data.Point2;
                        var deltaY = (pt1.y - pt2.y);
                        var deltaX = (pt2.x - pt1.x);
                        var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                        if (result < 0) { result = (result + 360); }
                        if (result >= 0 && result <= 89)
                        {
                            var thisPoint = new PointGeometry(new Point3D(pt2.x, pt2.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                        if (result >= 90 && result <= 179)
                        {
                            var thisPoint = new PointGeometry(new Point3D(pt1.x, pt1.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                        if (result >= 180 && result <= 269)
                        {
                            var thisPoint = new PointGeometry(new Point3D(pt2.x, pt2.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                        if (result >= 270 && result <= 359)
                        {
                            var thisPoint = new PointGeometry(new Point3D(pt1.x, pt1.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                    }
                }
                foreach (var point in secondPointList)
                {
                    var step = 0;
                    var newLine = new LineGeometry();
                    var thisPoint = PointGeometry.RetrieveEntity(point);
                    if (thisPoint is PointGeometry yesThisPoint)
                    {
                        var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                        foreach (var centerPoint in pointList)
                        {
                            var thisCenterPoint = Geometry.RetrieveEntity(centerPoint);
                            if (thisCenterPoint is PointGeometry yesThisCenterPoint)
                            {
                                var finallyThisCenterPoint = new Point3D(yesThisCenterPoint.Data.x, yesThisCenterPoint.Data.y, 0.0);
                                if (step == 0)
                                {
                                    newLine = new LineGeometry(finallyThisPoint, finallyThisCenterPoint);
                                    newLine.Commit();
                                    step = 1;
                                }
                                if (VectorManager.Distance(finallyThisPoint, finallyThisCenterPoint) < VectorManager.Distance(newLine.Data.Point1, newLine.Data.Point2) && step == 1)
                                {
                                    newLine.Data.Point2 = finallyThisCenterPoint;
                                    newLine.Color = 90;
                                    newLine.Commit();
                                }
                                lineList.Add(newLine.GetEntityID());
                            }
                        }
                    }
                }
                foreach (var point in pointList)
                {
                    var tempLineListDown = new List<double>();
                    var tempLinesGoingDown = new List<int>();
                    var tempLineListUp = new List<double>();
                    var tempLinesGoingUp = new List<int>();
                    if (Geometry.RetrieveEntity(point) is PointGeometry centerPoint)
                    {
                        var pointGeo = new Point3D(centerPoint.Data.x, centerPoint.Data.y, 0.0);
                        foreach (var entity in lineList)
                        {
                            if (Geometry.RetrieveEntity(entity) is LineGeometry line)
                            {
                                if (VectorManager.Distance(line.EndPoint2, pointGeo) <= 0.001)
                                {
                                    var pt1 = line.EndPoint1;
                                    var pt2 = line.EndPoint2;
                                    var deltaY = (pt1.y - pt2.y);
                                    var deltaX = (pt2.x - pt1.x);
                                    var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                                    if (result < 0) { result = (result + 360); }
                                    if (result >= 0 && result <= 179)
                                    {
                                        var thisLine = line.GetEntityID();
                                        if (tempLinesGoingDown.Contains(thisLine) == false) { tempLinesGoingDown.Add(thisLine); }
                                        if (tempLineListDown.Contains(VectorManager.Distance(pt1, pt2)) == false) { tempLineListDown.Add(VectorManager.Distance(pt1, pt2)); }
                                        if (tempLineListDown.Count == 4)
                                        {
                                            tempLineListDown.Sort();
                                            foreach (var extraLine in tempLinesGoingDown)
                                            {
                                                if (Geometry.RetrieveEntity(extraLine) is LineGeometry i)
                                                {
                                                    var lineLength = VectorManager.Distance(i.EndPoint1, i.EndPoint2);
                                                    if (lineLength == tempLineListDown[0])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 50;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListDown[1])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 51;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListDown[2])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 52;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListDown[3])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 53;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            tempLinesGoingDown.Clear();
                                            tempLineListDown.Clear();
                                        }
                                    }
                                }
                                if (VectorManager.Distance(line.EndPoint2, pointGeo) <= 0.001)
                                {
                                    var pt1 = line.EndPoint1;
                                    var pt2 = line.EndPoint2;
                                    var deltaY = (pt1.y - pt2.y);
                                    var deltaX = (pt2.x - pt1.x);
                                    var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                                    if (result < 0) { result = (result + 360); }
                                    if (result >= 180 && result <= 259)
                                    {
                                        var thisLine = line.GetEntityID();
                                        if (tempLinesGoingUp.Contains(thisLine) == false) { tempLinesGoingUp.Add(thisLine); }
                                        if (tempLineListUp.Contains(VectorManager.Distance(pt1, pt2)) == false) { tempLineListUp.Add(VectorManager.Distance(pt1, pt2)); }
                                        if (tempLineListUp.Count == 4)
                                        {
                                            tempLineListUp.Sort();
                                            foreach (var extraLine in tempLinesGoingUp)
                                            {
                                                if (Geometry.RetrieveEntity(extraLine) is LineGeometry i)
                                                {
                                                    var lineLength = VectorManager.Distance(i.EndPoint1, i.EndPoint2);
                                                    if (lineLength == tempLineListUp[0])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 50;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListUp[1])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 51;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListUp[2])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 52;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListUp[3])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 53;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            tempLinesGoingUp.Clear();
                                            tempLineListUp.Clear();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var line in templist500)
                {
                    var thisLine = Geometry.RetrieveEntity(line);
                    if (thisLine is LineGeometry noThisLine)
                    {
                        var pt1 = noThisLine.Data.Point1;
                        var pt2 = noThisLine.Data.Point2;
                        foreach (var endPoint in secondPointList)
                        {
                            if (Geometry.RetrieveEntity(endPoint) is PointGeometry pointy)
                            {
                                var point = new Point3D(pointy.Data.x, pointy.Data.y, 0.0);
                                if (VectorManager.Distance(pt1, point) <= 0.001 || (VectorManager.Distance(pt2, point) <= 0.001))
                                {
                                    noThisLine.Color = pointy.Color;
                                    noThisLine.Commit();
                                    pointy.Delete();
                                }
                            }
                        }
                    }
                }
                foreach (var line in templist500)
                {
                    var thisLine = Geometry.RetrieveEntity(line);
                    if (thisLine is LineGeometry noThisLine)
                    {
                        var pt1 = noThisLine.EndPoint1;
                        var pt2 = noThisLine.EndPoint2;
                        var deltaY = (pt1.y - pt2.y);
                        var deltaX = (pt2.x - pt1.x);
                        var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                        if (result < 0) { result = (result + 360); }
                        if (result >= 0 && result <= 89)
                        {
                            if (thisLine.Color == 50)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.115;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 11;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt2, -90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.699;
                                    newLine.Scale(newLine.EndPoint2, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 12;
                                    newLine.Commit();
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 51)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 13;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt1, 90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength;
                                    newLine.Scale(newLine.EndPoint2, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 14;
                                    newLine.Commit();
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 52)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 15;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt2, -90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.815;
                                    newLine.Scale(newLine.EndPoint2, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 16;
                                    newLine.Commit();
                                    var thisPoint = new PointGeometry(new Point3D(newLine.EndPoint1.x, newLine.EndPoint1.y, 0.0));
                                    thisPoint.Commit();
                                    finalPointList500.Add(thisPoint.GetEntityID());
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 53)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.0497;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 17;
                                thisLine.Commit();
                            }
                        }
                        if (result >= 90 && result <= 179)
                        {
                            if (thisLine.Color == 50)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.115;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 18;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt1, 90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.699;
                                    newLine.Scale(newLine.EndPoint1, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 19;
                                    newLine.Commit();
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 51)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 20;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt2, -90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength;
                                    newLine.Scale(newLine.EndPoint1, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 21;
                                    newLine.Commit();
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 52)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 22;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt1, 90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.815;
                                    newLine.Scale(newLine.EndPoint1, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 23;
                                    newLine.Commit();
                                    var thisPoint = new PointGeometry(new Point3D(newLine.EndPoint2.x, newLine.EndPoint2.y, 0.0));
                                    thisPoint.Commit();
                                    finalPointList500.Add(thisPoint.GetEntityID());
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 53)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.0497;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 24;
                                thisLine.Commit();
                            }
                        }
                        if (result >= 180 && result <= 269)
                        {
                            if (thisLine.Color == 50)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.115;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 25;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt1, 90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.699;
                                    newLine.Scale(newLine.EndPoint1, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 26;
                                    newLine.Commit();
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 51)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 27;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt2, -90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.735;
                                    newLine.Scale(newLine.EndPoint2, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 28;
                                    newLine.Commit();
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 52)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 29;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt2, -90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.815;
                                    newLine.Scale(newLine.EndPoint1, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 30;
                                    newLine.Commit();
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 53)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.0497;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 31;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                var thisPoint = new PointGeometry(new Point3D(pt2.x, pt2.y, 0.0));
                                thisPoint.Commit();
                                finalPointList500.Add(thisPoint.GetEntityID());
                            }
                        }
                        if (result >= 270 && result <= 359)
                        {
                            if (thisLine.Color == 50)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.115;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 32;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt1, 90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.699;
                                    newLine.Scale(newLine.EndPoint1, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 33;
                                    newLine.Commit();
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 51)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 34;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt1, 90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.735;
                                    newLine.Scale(newLine.EndPoint1, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 35;
                                    newLine.Commit();
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 52)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 36;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt1, 90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.815;
                                    newLine.Scale(newLine.EndPoint2, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 37;
                                    newLine.Commit();
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 53)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.0497;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 38;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                var thisPoint = new PointGeometry(new Point3D(pt1.x, pt1.y, 0.0));
                                thisPoint.Commit();
                                finalPointList500.Add(thisPoint.GetEntityID());
                            }
                        }
                    }
                }
                foreach (var point in finalPointList500)
                {
                    var step = 0;
                    var newLine = new LineGeometry();
                    var thisPoint = PointGeometry.RetrieveEntity(point);
                    if (thisPoint is PointGeometry yesThisPoint)
                    {
                        var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                        foreach (var secondPoint in finalPointList500)
                        {
                            if (secondPoint != point)
                            {
                                var thisSecondPoint = Geometry.RetrieveEntity(secondPoint);
                                if (thisSecondPoint is PointGeometry yesThisSecondPoint)
                                {
                                    var finallyThisSecondPoint = new Point3D(yesThisSecondPoint.Data.x, yesThisSecondPoint.Data.y, 0.0);
                                    if (step == 0)
                                    {
                                        newLine = new LineGeometry(finallyThisPoint, finallyThisSecondPoint);
                                        newLine.Commit();
                                        step = 1;
                                    }
                                    if (VectorManager.Distance(finallyThisPoint, finallyThisSecondPoint) < VectorManager.Distance(newLine.Data.Point1, newLine.Data.Point2) && step == 1)
                                    {
                                        newLine.Data.Point2 = finallyThisSecondPoint;
                                        newLine.Color = 90;
                                        newLine.Commit();
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var point in finalPointList500)
                {
                    var thisPoint = PointGeometry.RetrieveEntity(point);
                    thisPoint.Retrieve();
                    thisPoint.Delete();
                }
                foreach (var chain in ChainManager.ChainAll(500))
                {
                    var chainGeo = ChainManager.GetGeometryInChain(chain);
                    foreach (var entity in chainGeo)
                    {
                        entity.Color = 70;
                        entity.Selected = false;
                        entity.Commit();
                    }
                }
                SelectionManager.UnselectAllGeometry();
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                GraphicsManager.Repaint();
            }
            void findLineChainEnds501()
            {
                SelectionManager.UnselectAllGeometry();
                LevelsManager.RefreshLevelsManager();
                LevelsManager.SetMainLevel(501);
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown) { LevelsManager.SetLevelVisible(level, false); }
                LevelsManager.SetLevelVisible(501, true);
                LevelsManager.SetLevelVisible(50, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);
                SelectionManager.SelectGeometryByMask(Mastercam.IO.Types.QuickMaskType.Points);
                var selectedGeometry = SearchManager.GetSelectedGeometry();
                foreach (var point in selectedGeometry) { pointList.Add(point.GetEntityID()); }
                LevelsManager.SetLevelVisible(50, false);
                LevelsManager.RefreshLevelsManager();
                //GraphicsManager.Repaint(true);
                var chainDetails = new Mastercam.Database.Interop.ChainDetails();// Preps the ChainDetails plugin
                var selectedChains = ChainManager.ChainAll(501);
                var chainDirection = ChainDirectionType.CounterClockwise;// Going to be used to make sure all chains go the same direction
                ChainManager.StartChainAtLongest(selectedChains);

                foreach (var chain in selectedChains)
                {
                    chain.Direction = chainDirection;
                    var chainData = chainDetails.GetData(chain);
                    var firstEntity = chainData.FirstEntity.GetEntityID();
                    var lastEntity = chainData.LastEntity.GetEntityID();
                    var firstIsFlipped = chainData.FirstEntityIsFlipped;
                    var lastIsFlipped = chainData.LastEntityIsFlipped;
                    if (Geometry.RetrieveEntity(firstEntity) is LineGeometry startLine)
                    {
                        startLine.Color = 70;
                        startLine.Commit();
                        templist501.Add(firstEntity);
                    }
                    if (Geometry.RetrieveEntity(lastEntity) is LineGeometry endLine)
                    {
                        endLine.Color = 70;
                        endLine.Commit();
                        templist501.Add(lastEntity);
                    }
                }
                foreach (var line in templist501)
                {
                    var thisLine = Geometry.RetrieveEntity(line);
                    if (thisLine is LineGeometry noThisLine)
                    {
                        var pt1 = noThisLine.Data.Point1;
                        var pt2 = noThisLine.Data.Point2;
                        var deltaY = (pt1.y - pt2.y);
                        var deltaX = (pt2.x - pt1.x);
                        var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                        if (result < 0) { result = (result + 360); }
                        if (result >= 0 && result <= 89)
                        {
                            var thisPoint = new PointGeometry(new Point3D(pt2.x, pt2.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                        if (result >= 90 && result <= 179)
                        {
                            var thisPoint = new PointGeometry(new Point3D(pt1.x, pt1.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                        if (result >= 180 && result <= 269)
                        {
                            var thisPoint = new PointGeometry(new Point3D(pt2.x, pt2.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                        if (result >= 270 && result <= 359)
                        {
                            var thisPoint = new PointGeometry(new Point3D(pt1.x, pt1.y, 0.0));
                            thisPoint.Color = 90;
                            thisPoint.Commit();
                            secondPointList.Add(thisPoint.GetEntityID());
                        }
                    }
                }
                foreach (var point in secondPointList)
                {
                    var step = 0;
                    var newLine = new LineGeometry();
                    var thisPoint = PointGeometry.RetrieveEntity(point);
                    if (thisPoint is PointGeometry yesThisPoint)
                    {
                        var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                        foreach (var centerPoint in pointList)
                        {
                            var thisCenterPoint = Geometry.RetrieveEntity(centerPoint);
                            if (thisCenterPoint is PointGeometry yesThisCenterPoint)
                            {
                                var finallyThisCenterPoint = new Point3D(yesThisCenterPoint.Data.x, yesThisCenterPoint.Data.y, 0.0);
                                if (step == 0)
                                {
                                    newLine = new LineGeometry(finallyThisPoint, finallyThisCenterPoint);
                                    newLine.Commit();
                                    step = 1;
                                }
                                if (VectorManager.Distance(finallyThisPoint, finallyThisCenterPoint) < VectorManager.Distance(newLine.Data.Point1, newLine.Data.Point2) && step == 1)
                                {
                                    newLine.Data.Point2 = finallyThisCenterPoint;
                                    newLine.Color = 90;
                                    newLine.Commit();
                                }
                                lineList.Add(newLine.GetEntityID());
                            }
                        }
                    }
                }
                foreach (var point in pointList)
                {
                    var tempLineListDown = new List<double>();
                    var tempLinesGoingDown = new List<int>();
                    var tempLineListUp = new List<double>();
                    var tempLinesGoingUp = new List<int>();
                    if (Geometry.RetrieveEntity(point) is PointGeometry centerPoint)
                    {
                        var pointGeo = new Point3D(centerPoint.Data.x, centerPoint.Data.y, 0.0);
                        foreach (var entity in lineList)
                        {
                            if (Geometry.RetrieveEntity(entity) is LineGeometry line)
                            {
                                if (VectorManager.Distance(line.EndPoint2, pointGeo) <= 0.001)
                                {
                                    var pt1 = line.EndPoint1;
                                    var pt2 = line.EndPoint2;
                                    var deltaY = (pt1.y - pt2.y);
                                    var deltaX = (pt2.x - pt1.x);
                                    var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                                    if (result < 0) { result = (result + 360); }
                                    if (result >= 0 && result <= 179)
                                    {
                                        var thisLine = line.GetEntityID();
                                        if (tempLinesGoingDown.Contains(thisLine) == false) { tempLinesGoingDown.Add(thisLine); }
                                        if (tempLineListDown.Contains(VectorManager.Distance(pt1, pt2)) == false) { tempLineListDown.Add(VectorManager.Distance(pt1, pt2)); }
                                        if (tempLineListDown.Count == 4)
                                        {
                                            tempLineListDown.Sort();
                                            foreach (var extraLine in tempLinesGoingDown)
                                            {
                                                if (Geometry.RetrieveEntity(extraLine) is LineGeometry i)
                                                {
                                                    var lineLength = VectorManager.Distance(i.EndPoint1, i.EndPoint2);
                                                    if (lineLength == tempLineListDown[0])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 50;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListDown[1])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 51;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListDown[2])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 52;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListDown[3])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 53;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            tempLinesGoingDown.Clear();
                                            tempLineListDown.Clear();
                                        }
                                    }
                                }
                                if (VectorManager.Distance(line.EndPoint2, pointGeo) <= 0.001)
                                {
                                    var pt1 = line.EndPoint1;
                                    var pt2 = line.EndPoint2;
                                    var deltaY = (pt1.y - pt2.y);
                                    var deltaX = (pt2.x - pt1.x);
                                    var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                                    if (result < 0) { result = (result + 360); }
                                    if (result >= 180 && result <= 259)
                                    {
                                        var thisLine = line.GetEntityID();
                                        if (tempLinesGoingUp.Contains(thisLine) == false) { tempLinesGoingUp.Add(thisLine); }
                                        if (tempLineListUp.Contains(VectorManager.Distance(pt1, pt2)) == false) { tempLineListUp.Add(VectorManager.Distance(pt1, pt2)); }
                                        if (tempLineListUp.Count == 4)
                                        {
                                            tempLineListUp.Sort();
                                            foreach (var extraLine in tempLinesGoingUp)
                                            {
                                                if (Geometry.RetrieveEntity(extraLine) is LineGeometry i)
                                                {
                                                    var lineLength = VectorManager.Distance(i.EndPoint1, i.EndPoint2);
                                                    if (lineLength == tempLineListUp[0])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 50;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListUp[1])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 51;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListUp[2])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 52;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lineLength == tempLineListUp[3])
                                                    {
                                                        i.Delete();
                                                        foreach (var endPoint in secondPointList)
                                                        {
                                                            var thisPoint = PointGeometry.RetrieveEntity(endPoint);
                                                            if (thisPoint is PointGeometry yesThisPoint)
                                                            {
                                                                var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                                                                if (VectorManager.Distance(i.EndPoint1, finallyThisPoint) <= 0.001)
                                                                {
                                                                    thisPoint.Color = 53;
                                                                    thisPoint.Commit();
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            tempLinesGoingUp.Clear();
                                            tempLineListUp.Clear();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var line in templist501)
                {
                    var thisLine = Geometry.RetrieveEntity(line);
                    if (thisLine is LineGeometry noThisLine)
                    {
                        var pt1 = noThisLine.Data.Point1;
                        var pt2 = noThisLine.Data.Point2;
                        foreach (var endPoint in secondPointList)
                        {
                            if (Geometry.RetrieveEntity(endPoint) is PointGeometry pointy)
                            {
                                var point = new Point3D(pointy.Data.x, pointy.Data.y, 0.0);
                                if (VectorManager.Distance(pt1, point) <= 0.001 || (VectorManager.Distance(pt2, point) <= 0.001))
                                {
                                    noThisLine.Color = pointy.Color;
                                    noThisLine.Commit();
                                    pointy.Delete();
                                }
                            }
                        }
                    }
                }
                foreach (var line in templist501)
                {
                    var thisLine = Geometry.RetrieveEntity(line);
                    if (thisLine is LineGeometry noThisLine)
                    {
                        var pt1 = noThisLine.EndPoint1;
                        var pt2 = noThisLine.EndPoint2;
                        var deltaY = (pt1.y - pt2.y);
                        var deltaX = (pt2.x - pt1.x);
                        var result = VectorManager.RadiansToDegrees(Math.Atan2(deltaY, deltaX));
                        if (result < 0) { result = (result + 360); }
                        if (result >= 0 && result <= 89)
                        {
                            if (thisLine.Color == 50)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0125;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 11;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                            }
                            if (thisLine.Color == 51)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 12;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                            }
                            if (thisLine.Color == 52)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 13;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt1, 90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.815;
                                    newLine.Scale(newLine.EndPoint1, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 14;
                                    newLine.Commit();
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 53)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.034;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 15;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                var thisPoint = new PointGeometry(new Point3D(pt2.x, pt2.y, 0.0));
                                thisPoint.Commit();
                                finalPointList501.Add(thisPoint.GetEntityID());
                            }
                        }
                        if (result >= 90 && result <= 179)
                        {
                            if (thisLine.Color == 50)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0125;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 16;
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 51)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 17;
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 52)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 18;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt1, 90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.815;
                                    newLine.Scale(newLine.EndPoint1, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 19;
                                    newLine.Commit();
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 53)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.034;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 20;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                var thisPoint = new PointGeometry(new Point3D(pt1.x, pt1.y, 0.0));
                                thisPoint.Commit();
                                finalPointList501.Add(thisPoint.GetEntityID());
                            }
                        }
                        if (result >= 180 && result <= 269)
                        {
                            if (thisLine.Color == 50)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0125;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 21;
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 51)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 22;
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 52)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 23;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt2, -90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.815;
                                    newLine.Scale(newLine.EndPoint2, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 24;
                                    newLine.Commit();
                                    var thisPoint = new PointGeometry(new Point3D(newLine.EndPoint1.x, newLine.EndPoint1.y, 0.0));
                                    thisPoint.Commit();
                                    finalPointList501.Add(thisPoint.GetEntityID());
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 53)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.034;
                                thisLine.Scale(pt1, (newLineLength / lineLength));
                                thisLine.Color = 25;
                                thisLine.Commit();
                            }
                        }
                        if (result >= 270 && result <= 359)
                        {
                            if (thisLine.Color == 50)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0125;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 26;
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 51)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0325;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 27;
                                thisLine.Commit();
                            }
                            if (thisLine.Color == 52)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength + 0.0075;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 28;
                                thisLine.Selected = true;
                                thisLine.Commit();
                                noThisLine.Retrieve();
                                pt1 = noThisLine.EndPoint1;
                                pt2 = noThisLine.EndPoint2;
                                GeometryManipulationManager.RotateGeometry(pt1, 90, Top, true);
                                var additionalLine = SearchManager.GetResultGeometry();
                                if (additionalLine[0] is LineGeometry newLine)
                                {
                                    var additionalLineLength = VectorManager.Distance(newLine.EndPoint1, newLine.EndPoint2);
                                    var newAdditionalLineLength = additionalLineLength - 1.815;
                                    newLine.Scale(newLine.EndPoint1, newAdditionalLineLength / additionalLineLength);
                                    newLine.Color = 29;
                                    newLine.Commit();
                                    var thisPoint = new PointGeometry(new Point3D(newLine.EndPoint2.x, newLine.EndPoint2.y, 0.0));
                                    thisPoint.Commit();
                                    finalPointList501.Add(thisPoint.GetEntityID());
                                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                    additionalLine = null;
                                }
                            }
                            if (thisLine.Color == 53)
                            {
                                var lineLength = VectorManager.Distance(pt1, pt2);
                                var newLineLength = lineLength - 0.034;
                                thisLine.Scale(pt2, (newLineLength / lineLength));
                                thisLine.Color = 30;
                                thisLine.Commit();
                            }
                        }
                    }
                }
                foreach (var point in finalPointList501)
                {
                    var step = 0;
                    var newLine = new LineGeometry();
                    var thisPoint = PointGeometry.RetrieveEntity(point);
                    if (thisPoint is PointGeometry yesThisPoint)
                    {
                        var finallyThisPoint = new Point3D(yesThisPoint.Data.x, yesThisPoint.Data.y, 0.0);
                        foreach (var secondPoint in finalPointList501)
                        {
                            if (secondPoint != point)
                            {
                                var thisSecondPoint = Geometry.RetrieveEntity(secondPoint);
                                if (thisSecondPoint is PointGeometry yesThisSecondPoint)
                                {
                                    var finallyThisSecondPoint = new Point3D(yesThisSecondPoint.Data.x, yesThisSecondPoint.Data.y, 0.0);
                                    if (step == 0)
                                    {
                                        newLine = new LineGeometry(finallyThisPoint, finallyThisSecondPoint);
                                        newLine.Commit();
                                        step = 1;
                                    }
                                    if (VectorManager.Distance(finallyThisPoint, finallyThisSecondPoint) < VectorManager.Distance(newLine.Data.Point1, newLine.Data.Point2) && step == 1)
                                    {
                                        newLine.Data.Point2 = finallyThisSecondPoint;
                                        newLine.Color = 90;
                                        newLine.Commit();
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var point in finalPointList501)
                {
                    var thisPoint = PointGeometry.RetrieveEntity(point);
                    thisPoint.Retrieve();
                    thisPoint.Delete();
                }
                foreach (var chain in ChainManager.ChainAll(501))
                {
                    var chainGeo = ChainManager.GetGeometryInChain(chain);
                    foreach (var entity in chainGeo)
                    {
                        entity.Color = 70;
                        entity.Selected = false;
                        entity.Commit();
                    }
                }
                SelectionManager.UnselectAllGeometry();
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                GraphicsManager.Repaint();
            }
            void deSelect()
            {
                var selectedGeo = SearchManager.GetGeometry();
                foreach (var entity in selectedGeo)
                {
                    entity.Retrieve();
                    entity.Selected = false;
                    entity.Commit();
                }
            }
            bool CreateLine1()
            {
                bool result = false;
                Point3D crease1 = new Point3D(creaseX1, creaseY1, 0.0);
                Point3D crease3 = new Point3D(creaseX3, creaseY3, 0.0);
                LineGeometry Line1 = new LineGeometry(crease1, crease3);
                Line1.Selected = true;
                result = Line1.Commit();
                return result;
            } // Used for one line at end of crease
            bool CreateLine2()
            {
                bool result = false;
                Point3D crease2 = new Point3D(creaseX2, creaseY2, 0.0);
                Point3D crease4 = new Point3D(creaseX4, creaseY4, 0.0);
                LineGeometry Line2 = new LineGeometry(crease2, crease4);
                Line2.Selected = true;
                result = Line2.Commit();
                return result;
            } // Used for other line at other end of crease
            void offsetCreasechain()
            {
                //Unselects all Geometry
                SelectionManager.UnselectAllGeometry();
                //Turns off all visible levels
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown)
                {
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.RefreshLevelsManager();
                //Sets level 101 to main level and visible
                LevelsManager.SetMainLevel(101);
                LevelsManager.SetLevelVisible(101, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);//required

                //Selects all geometry on level 101
                var selectedCreaseChain = ChainManager.ChainAll(101);
                //Creates and names level 502 and level 503
                LevelsManager.SetLevelName(502, "Upper Created Crease Geo");
                LevelsManager.SetLevelName(503, "Lower Created Crease Geo");

                //edits each entity of all chains
                foreach (var chain in selectedCreaseChain)
                {
                    //offsets line of lower
                    var lowerChainCrease1 = chain.OffsetChain2D(OffsetSideType.Left, .040, OffsetRollCornerType.None, .5, false, .005, false);
                    var lowerChainCrease2 = chain.OffsetChain2D(OffsetSideType.Left, .065, OffsetRollCornerType.None, .5, false, .005, false);
                    var lowerChainCrease3 = chain.OffsetChain2D(OffsetSideType.Right, .040, OffsetRollCornerType.None, .5, false, .005, false);
                    var lowerChainCrease4 = chain.OffsetChain2D(OffsetSideType.Right, .065, OffsetRollCornerType.None, .5, false, .005, false);
                    //Colors and selects result geometry
                    var creaseResultGeometry = SearchManager.GetResultGeometry();
                    foreach (var entity in creaseResultGeometry)
                    {
                        lowerCreaseID.Add(entity.GetEntityID());
                        entity.Color = 11;
                        entity.Level = createdLowerCrease;
                        entity.Selected = false;
                        entity.Commit();
                    }
                    //Clears geometry in result
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));

                    //offsets line of upper
                    var upperChainCrease1 = chain.OffsetChain2D(OffsetSideType.Left, .014, OffsetRollCornerType.None, .5, false, .005, false);
                    var upperChainCrease2 = chain.OffsetChain2D(OffsetSideType.Right, .014, OffsetRollCornerType.None, .5, false, .005, false);
                    //Colors and selects result geometry
                    var creaseResultGeometryNew = SearchManager.GetResultGeometry();
                    foreach (var entity in creaseResultGeometryNew)
                    {
                        upperCreaseID.Add(entity.GetEntityID());
                        entity.Color = 10;
                        entity.Level = createdUpperCrease;
                        entity.Selected = true;
                        entity.Commit();
                    }
                    //Clears geometry in result
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                }
            } // Offsets creases
            void connectUpperLines()
            {
                var set = 0;
                foreach (var i in upperCreaseID)
                {
                    if (set == 0)
                    {
                        var creaseGeo = Geometry.RetrieveEntity(i);
                        var line = (LineGeometry)creaseGeo;
                        creaseX1 = line.EndPoint1.x;
                        creaseX2 = line.EndPoint2.x;
                        creaseY1 = line.EndPoint1.y;
                        creaseY2 = line.EndPoint2.y;
                        set = 1;
                    }
                    else
                    {
                        var creaseGeo2 = Geometry.RetrieveEntity(i);
                        var line2 = (LineGeometry)creaseGeo2;
                        creaseX3 = line2.EndPoint1.x;
                        creaseX4 = line2.EndPoint2.x;
                        creaseY3 = line2.EndPoint1.y;
                        creaseY4 = line2.EndPoint2.y;
                        CreateLine1();
                        CreateLine2();

                        var creaseResultGeometryNew = SearchManager.GetSelectedGeometry();
                        foreach (var entity in creaseResultGeometryNew)
                        {
                            entity.Color = 10;
                            entity.Level = createdUpperCrease;
                            entity.Selected = false;
                            entity.Commit();
                        }
                        //Moves result geometry
                        //Clears geometry in result
                        GraphicsManager.ClearColors(new GroupSelectionMask(true));
                        //Deselects all
                        SelectionManager.UnselectAllGeometry();

                        set = 0;
                    }
                }
            } // Connects crease offsets for the upper
            void connectLowerLines()
            {
                var set = 0;
                foreach (var i in lowerCreaseID)
                {
                    if (set == 0)
                    {
                        var creaseGeo = Geometry.RetrieveEntity(i);
                        var line = (LineGeometry)creaseGeo;
                        creaseX1 = line.EndPoint1.x;
                        creaseX2 = line.EndPoint2.x;
                        creaseY1 = line.EndPoint1.y;
                        creaseY2 = line.EndPoint2.y;
                        set = 1;
                    }
                    else
                    {
                        var creaseGeo2 = Geometry.RetrieveEntity(i);
                        var line2 = (LineGeometry)creaseGeo2;
                        creaseX3 = line2.EndPoint1.x;
                        creaseX4 = line2.EndPoint2.x;
                        creaseY3 = line2.EndPoint1.y;
                        creaseY4 = line2.EndPoint2.y;
                        CreateLine1();
                        CreateLine2();

                        var creaseResultGeometryNew = SearchManager.GetSelectedGeometry();
                        foreach (var entity in creaseResultGeometryNew)
                        {
                            entity.Color = 11;
                            entity.Level = createdLowerCrease;
                            entity.Selected = false;
                            entity.Commit();
                        }
                        //Moves result geometry

                        //Clears geometry in result
                        GraphicsManager.ClearColors(new GroupSelectionMask(true));
                        //Deselects all
                        SelectionManager.UnselectAllGeometry();

                        set = 0;
                    }
                }
            } // Connects crease offsets for the lower
            void DemoAlterLine()
            {
                //Unselects all Geometry
                SelectionManager.UnselectAllGeometry();
                //Turns off all visible levels
                var shown = LevelsManager.GetVisibleLevelNumbers();
                foreach (var level in shown)
                {
                    LevelsManager.SetLevelVisible(level, false);
                }
                LevelsManager.RefreshLevelsManager();
                //Sets level 101 to main level and visible
                LevelsManager.SetMainLevel(101);
                LevelsManager.SetLevelVisible(101, true);
                LevelsManager.RefreshLevelsManager();
                //GraphicsManager.Repaint(true);

                var geomask = new GeometryMask { Lines = true };
                var geoSel = new SelectionMask { };
                var geo = SearchManager.GetGeometry(geomask, geoSel, 101);
                if (geo != null)
                {
                    //Selects each line. Determines orientation and alters by 1 inch from both ends
                    foreach (var singleGeo in geo)
                    {
                        var line = (LineGeometry)singleGeo;
                        //If line is vertical
                        if (line.Data.Point1.x == line.Data.Point2.x)
                        {
                            if (line.Data.Point1.y >= line.Data.Point2.y)
                            {
                                line.Data.Point1.y += -0.07;
                                line.Data.Point2.y += +0.07;
                                line.Selected = false;
                            };
                            if (line.Data.Point1.y <= line.Data.Point2.y)
                            {
                                line.Data.Point1.y += +0.07;
                                line.Data.Point2.y += -0.07;
                                line.Selected = false;
                            };
                        };
                        //If line is horizontal
                        if (line.Data.Point1.y == line.Data.Point2.y)
                        {
                            if (line.Data.Point1.x >= line.Data.Point2.x)
                            {
                                line.Data.Point1.x += -0.07;
                                line.Data.Point2.x += +0.07;
                                line.Selected = false;
                            };
                            if (line.Data.Point1.x <= line.Data.Point2.x)
                            {
                                line.Data.Point1.x += +0.07;
                                line.Data.Point2.x += -0.07;
                                line.Selected = false;
                            };
                        };
                        //Stores result in Mastercam
                        line.Commit();
                    }
                    //Updates screen shown
                    //GraphicsManager.Repaint(true);
                }
            } // Shortens creases

            deSelect();
            DemoAlterLine();
            deSelect();
            offsetCreasechain();
            deSelect();
            connectUpperLines();
            deSelect();
            connectLowerLines();
            deSelect();
            GraphicsManager.Repaint(true);
            deSelect();
            translate();
            deSelect();
            findIntersectionOfLines();
            deSelect();
            findIntersectionOfArcs();
            deSelect();
            clearTempLinesAndArcs();
            deSelect();
            breakArcsWithPoints();
            deSelect();
            selectNewGeo();
            deSelect();
            colorCrossovers();
            deSelect();
            movePoints();
            deSelect();
            seperateGeometry();
            deSelect();
            ChainManager.ChainTolerance = 0.0001;
            offsetCutchain80();
            deSelect();
            offsetCutchain81();
            deSelect();
            shortenChains500();
            deSelect();
            findLineChainEnds500();
            deSelect();
            findArcChainEnds500();
            deSelect();
            shortenChains501();
            deSelect();
            findArcChainEnds501();
            deSelect();
            findLineChainEnds501();
            deSelect();

            GraphicsManager.Repaint(true);
            return MCamReturn.NoErrors;
        }
    }
}

        
    


            

                



            

        


    


/*

void offsetCutchain();
{

    var selectedChain = ChainManager.ChainAll();
    int createdUpperLevel = 500;
    int createdLowerLevel = 501;
    LevelsManager.SetLevelName(500, "Upper Created Geo");
    LevelsManager.SetLevelName(501, "Lower Created Geo");

    foreach (var chain in selectedChain)
    {


        var lowerChainLarge = chain.OffsetChain2D(OffsetSideType.Left, .0225, OffsetRollCornerType.None, .5, false, .005, false);
        var lowerLargeGeometry = ChainManager.GetGeometryInChain(lowerChainLarge);

        var lowerChainSmall = chain.OffsetChain2D(OffsetSideType.Right, .0025, OffsetRollCornerType.None, .5, false, .005, false);
        var lowerSmallGeometry = ChainManager.GetGeometryInChain(lowerChainSmall);

        var resultGeometry = SearchManager.GetResultGeometry();
        foreach (var entity in resultGeometry)
        {
            entity.Color = 11;
            entity.Selected = true;
            entity.Commit();
        }
        GeometryManipulationManager.MoveSelectedGeometryToLevel(createdLowerLevel, true);
        GraphicsManager.ClearColors(new GroupSelectionMask(true));

        var upperChainLarge = chain.OffsetChain2D(OffsetSideType.Left, .0025, OffsetRollCornerType.None, .5, false, .005, false);
        var upperLargeGeometry = ChainManager.GetGeometryInChain(upperChainLarge);

        var upperChainSmall = chain.OffsetChain2D(OffsetSideType.Right, .0385, OffsetRollCornerType.None, .5, false, .005, false);
        var upperSmallGeometry = ChainManager.GetGeometryInChain(upperChainSmall);

        var resultGeometryNew = SearchManager.GetResultGeometry();
        foreach (var entity in resultGeometryNew)
        {
            entity.Color = 10;
            entity.Selected = true;
            entity.Commit();
        }
        GeometryManipulationManager.MoveSelectedGeometryToLevel(createdUpperLevel, true);
        GraphicsManager.ClearColors(new GroupSelectionMask(true));

    }


}





// Working Offset Chain
/*

var selectedChain = ChainManager.GetOneChain("Select a Chain");

var offsetChain = selectedChain.OffsetChain2D(OffsetSideType.Left,
                                              .245,
                                              OffsetRollCornerType.None,
                                              .5,
                                              false,
                                              .005,
                                              false);

var offsetGeometry = ChainManager.GetGeometryInChain(offsetChain);

foreach (var entity in offsetGeometry)
{
    entity.Commit();
}

return MCamReturn.NoErrors;
*/





//Working Translate
/*
bool MoveLine() {
    bool result = false;
    //Mastercam.IO.SelectionManager.SelectAllGeometry();
    Mastercam.Math.Point3D pt1 = new Mastercam.Math.Point3D(0.0, 0.0, 0.0);
    Mastercam.Math.Point3D pt2 = new Mastercam.Math.Point3D(100.0, 0.0, 0.0);
    MCView Top = new MCView();
    Mastercam.GeometryUtility.GeometryManipulationManager.TranslateGeometry(pt1, pt2, Top , Top, false);
    return result;
}
MoveLine();
*/


// working form
/*
var m = new Form1();
m.Show();
*/

// working line creation
/*
bool CreateLine()
{
    bool result = false;

    Mastercam.Math.Point3D pt1 = new Mastercam.Math.Point3D(0.0, 0.0, 0.0);
    Mastercam.Math.Point3D pt2 = new Mastercam.Math.Point3D(100.0, 0.0, 0.0);
    Mastercam.Curves.LineGeometry Line1 = new Mastercam.Curves.LineGeometry(pt1, pt2);
    result = Line1.Commit();
    result = Line1.Validate(); // Not really needed here, if Commit was successful - we're good!
                               //Mastercam.IO.GraphicsManager.Repaint(True)

    return result;
}
CreateLine();
*/

//working popup message
//  System.Windows.Forms.MessageBox.Show("Jeremy can make pop up messages!");
//return Mastercam.App.Types.MCamReturn.NoErrors;
