using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Windows.Forms;
using CustomNethook;
using Mastercam.Database;
using System.Drawing;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Mastercam.Math;
using Mastercam.IO;
using Mastercam.Database.Types;
using System.Numerics;
using System.Security.Policy;
using Mastercam.GeometryUtility.Types;
using Mastercam.App.Types;
using Mastercam.GeometryUtility;
using Mastercam.Support;
using System.ComponentModel.Design;
using System.Management.Instrumentation;
using Mastercam.Curves;
using Mastercam.IO.Types;
using System.Runtime.Remoting.Lifetime;
using Mastercam.BasicGeometry;
using System.Security.Cryptography;

namespace _CustomNethook
{
    public class CustomNethook : Mastercam.App.NetHook3App
    {
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
            void translate(){
                SelectionManager.UnselectAllGeometry();
                var tempChain1 = SearchManager.SelectAllGeometryOnLevel(75, true);
                var tempSelectedGeometry1 = SearchManager.GetSelectedGeometry();
                foreach (var i in tempSelectedGeometry1){
                    tempList8.Add(i.GetEntityID());
                }
                var temp1 = GeometryManipulationManager.CopySelectedGeometryToLevel(10, true);
                SelectionManager.UnselectAllGeometry();
                var tempChain2 = SearchManager.SelectAllGeometryOnLevel(75, true);
                var tempSelectedGeometry2 = SearchManager.GetSelectedGeometry();
                var temp2 = GeometryManipulationManager.CopySelectedGeometryToLevel(11, true);
                Point3D startPoint = new Point3D(0.0, 0.0, 0.0);
                Point3D endPointUpper = new Point3D(-0.001, -0.001, 0.0);
                Point3D endPointLower = new Point3D(0.001, 0.001, 0.0);
                Point3D resultPoint = new Point3D(-0.001, 0.001, 0.0);
                MCView Top = new MCView();
                foreach (var i in tempSelectedGeometry1){
                    tempList3.Add(i.GetEntityID());
                }

                var temp1Geometry = SearchManager.SelectAllGeometryOnLevel(10, true);
                var tempSelectedGeometry1Result = SearchManager.GetSelectedGeometry();
                foreach (var entity in tempSelectedGeometry1Result){
                    entity.Color = 10;
                    entity.Translate(startPoint, endPointUpper, Top, Top);
                    tempList1.Add(entity.GetEntityID());
                    entity.Commit();
                }
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                SelectionManager.UnselectAllGeometry();


                var temp2Geometry = SearchManager.SelectAllGeometryOnLevel(11, true);
                var tempSelectedGeometry2Result = SearchManager.GetSelectedGeometry();
                foreach (var entity in tempSelectedGeometry2Result){
                    entity.Color = 11;
                    entity.Translate(startPoint, endPointLower, Top, Top);
                    tempList2.Add(entity.GetEntityID());
                    entity.Commit();
                }
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                SelectionManager.UnselectAllGeometry();








            } // Moves line in X+Y+ direction and in X-Y- direction
            void findIntersectionOfArcs(){
                SelectionManager.UnselectAllGeometry();
                foreach (var v in tempList1){
                    var ArcID = Geometry.RetrieveEntity(v);
                    if (ArcID is ArcGeometry arc){
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
                        foreach (var u in tempList2){
                            var secondArcID = Geometry.RetrieveEntity(u);
                            if (secondArcID is ArcGeometry secondArc){
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
                                if (C1C2 < arc1Rad + arc2Rad){
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
                                    if (pt3degreeNeg < 0.0){
                                        pt3degreeNeg = (pt3degreeNeg + 360);
                                    }
                                    if (pt4degreeNeg < 0.0){
                                        pt4degreeNeg = (pt4degreeNeg + 360);
                                    }
                                    if (((arc2StartDegrees < pt3degreeNeg) && (pt3degreeNeg < arc2EndDegrees)) || ((pt3degreeNeg < arc2StartDegrees) && (arc2EndDegrees < pt3degreeNeg))){
                                        var point3 = new Mastercam.BasicGeometry.PointGeometry(pt3);
                                        point3.Color = 94;
                                        point3.Commit();
                                        tempList4.Add(point3.GetEntityID());
                                        tempList9.Add(point3.GetEntityID());
                                    }
                                    if (((arc2StartDegrees < pt4degreeNeg) && (pt4degreeNeg < arc2EndDegrees)) || ((pt4degreeNeg < arc2StartDegrees) && (arc2EndDegrees < pt4degreeNeg))){
                                        var point4 = new Mastercam.BasicGeometry.PointGeometry(pt4);
                                        point4.Color = 94;
                                        point4.Commit();
                                        tempList9.Add(point4.GetEntityID());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            void findIntersectionOfLines()
            {
                SelectionManager.UnselectAllGeometry();
                Point3D startPoint = new Point3D(0.0, 0.0, 0.0);
                Point3D resultPointUp = new Point3D(-0.001, 0.001, 0.0);
                Point3D resultPointDown = new Point3D(0.001, -0.001, 0.0);
                MCView Top = new MCView();
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
                                                newPoint.Translate(startPoint, resultPointUp, Top, Top);
                                                newPoint.Commit();
                                                tempList9.Add(newPoint.GetEntityID());
                                                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                                            }
                                            if (line1X1 >= pt1.x && pt1.x >= line2X1)
                                            { // if needs to shift down and right
                                                var newPoint = new Mastercam.BasicGeometry.PointGeometry(pt1);
                                                newPoint.Color = 94;
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
            }
            void clearTempLinesAndArcs(){
                foreach (var i in tempList1){
                    var tempI = Geometry.RetrieveEntity(i);
                    tempI.Delete();
                }
                foreach (var i in tempList2){
                    var tempI = Geometry.RetrieveEntity(i);
                    tempI.Delete();
                }
                GraphicsManager.ClearColors(new GroupSelectionMask(true));
                GraphicsManager.Repaint(true);
            }
            void breakArcsWithPoints(){
                foreach (var i in tempList3){
                    var ArcID = Geometry.RetrieveEntity(i);
                    if (ArcID is ArcGeometry arc){
                        var arc1X1 = arc.EndPoint1.x; // Arc x 1
                        var arc1X2 = arc.EndPoint2.x; // Arc x 2
                        var arc1Y1 = arc.EndPoint1.y; // Arc y 1
                        var arc1Y2 = arc.EndPoint2.y; // Arc y 2
                        var arc1Center = arc.Data.CenterPoint;
                        Point3D arc1StartPoint = new Point3D(arc1X1, arc1Y1, 0.0);
                        Point3D arc1EndPoint = new Point3D(arc1X2, arc1Y2, 0.0);
                        var tolerance = 0.001;
                        foreach (var v in tempList4){
                            var u = PointGeometry.RetrieveEntity(v);
                            if (u is PointGeometry point){
                                var ux = point.Data.x;
                                var uy = point.Data.y;
                                Point3D uPoint = new Point3D(uy, ux, 0.0);
                                var delta3Xneg = (arc1Center.x - uPoint.x);
                                var delta3Yneg = (arc1Center.y - uPoint.y);
                                var delta3Radneg = Math.Atan2(delta3Xneg, delta3Yneg);
                                var pt3degreeNeg = VectorManager.RadiansToDegrees(delta3Radneg);
                                if (pt3degreeNeg < 0.0){
                                    pt3degreeNeg = (pt3degreeNeg + 360);
                                }
                                var startToUDist = Math.Abs(VectorManager.Distance(arc1StartPoint, uPoint));
                                var endToUDist = Math.Abs(VectorManager.Distance(arc1EndPoint, uPoint));
                                if ((startToUDist >= (endToUDist - tolerance)) && (startToUDist <= (endToUDist + tolerance))){
                                    BreakManyPiecesParameters broken = new BreakManyPiecesParameters(){
                                        Curves = true,
                                        Number = 2,
                                        Method = 0
                                    };
                                    tempList7.Add(arc);
                                    GeometryManipulationManager.BreakManyPieces(tempList7,broken,ref tempList5,ref tempList6);
                                }
                            }
                        }
                    }
                }
            }
            void colorCrossovers(){
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
                var nextarcX1 = 0.0;
                var nextarcX2 = 0.0;
                var nextarcY1 = 0.0;
                var nextarcY2 = 0.0;
                Point3D nextarcStartPoint = new Point3D(nextarcX1, nextarcY1, 0.0);
                Point3D nextarcEndPoint = new Point3D(nextarcX2, nextarcY2, 0.0);
                var nextlineX1 = 0.0;
                var nextlineX2 = 0.0;
                var nextlineY1 = 0.0;
                var nextlineY2 = 0.0;
                Point3D nextlineStartPoint = new Point3D(nextlineX1, nextlineY1, 0.0);
                Point3D nextlineEndPoint = new Point3D(nextlineX2, nextlineY2, 0.0);
                foreach (var i in tempList8){
                    var GeoID = Geometry.RetrieveEntity(i);
                    var firstEntity = 0;
                    var secondEntity = 0;

                    if (GeoID is ArcGeometry arc){ // if first entity is arc
                        arcX1 = arc.EndPoint1.x; // Arc 1 x 1
                        arcX2 = arc.EndPoint2.x; // Arc 1 x 2
                        arcY1 = arc.EndPoint1.y; // Arc 1 y 1
                        arcY2 = arc.EndPoint2.y; // Arc 1 y 2
                        arcStartPoint = new Point3D(arcX1, arcY1, 0.0);
                        arcEndPoint = new Point3D(arcX2, arcY2, 0.0);
                        firstEntity = 1;
                    }

                    if (GeoID is LineGeometry line){ // if first entity is line
                        lineX1 = line.EndPoint1.x; // line 1 x 1
                        lineX2 = line.EndPoint2.x; // line 1 x 2
                        lineY1 = line.EndPoint1.y; // line 1 y 1
                        lineY2 = line.EndPoint2.y; // line 1 y 2
                        lineStartPoint = new Point3D(lineX1, lineY1, 0.0);
                        lineEndPoint = new Point3D(lineX2, lineY2, 0.0);
                        firstEntity = 2;
                    }

                    foreach (var u in tempList8){
                        var nextGeoID = Geometry.RetrieveEntity(u);

                        if (nextGeoID is ArcGeometry nextarc){ // if second entity is arc
                            nextarcX1 = nextarc.EndPoint1.x; // Arc 2 x 1
                            nextarcX2 = nextarc.EndPoint2.x; // Arc 2 x 2
                            nextarcY1 = nextarc.EndPoint1.y; // Arc 2 y 1
                            nextarcY2 = nextarc.EndPoint2.y; // Arc 2 y 2
                            nextarcStartPoint = new Point3D(nextarcX1, nextarcY1, 0.0);
                            nextarcEndPoint = new Point3D(nextarcX2, nextarcY2, 0.0);
                            secondEntity = 1;
                        }

                        if (nextGeoID is LineGeometry nextline){ // if second entity is line
                            nextlineX1 = nextline.EndPoint1.x; // line 2 x 1
                            nextlineX2 = nextline.EndPoint2.x; // line 2 x 2
                            nextlineY1 = nextline.EndPoint1.y; // line 2 y 1
                            nextlineY2 = nextline.EndPoint2.y; // line 2 y 2
                            nextlineStartPoint = new Point3D(nextlineX1, nextlineY1, 0.0);
                            nextlineEndPoint = new Point3D(nextlineX2, nextlineY2, 0.0);
                            secondEntity = 2;
                        }

                        if ((firstEntity == 1) && (secondEntity == 1)){
                            foreach (var v in tempList9){
                                var w = PointGeometry.RetrieveEntity(v);
                                if (w is PointGeometry point){
                                    var ux = point.Data.x;
                                    var uy = point.Data.y;
                                    Point3D uPoint = new Point3D(ux, uy, 0.0);
                                    var startDistance = Math.Abs(VectorManager.Distance(uPoint, arcStartPoint));
                                    var endDistance = Math.Abs(VectorManager.Distance(uPoint, arcEndPoint));
                                    if (startDistance <= tolerance){
                                        GeoID.Color = 10;
                                        GeoID.Commit();
                                    }
                                }
                            }
                        }//If arc and arc
                        if ((firstEntity == 1) && (secondEntity == 2)){

                        }//If arc and line
                        if ((firstEntity == 2) && (secondEntity == 1)){

                        }//If line and arc
                        if ((firstEntity == 2) && (secondEntity == 2)){

                        }//If line and line
                    }
                }
            }
            translate();
            findIntersectionOfLines();
            findIntersectionOfArcs();
            clearTempLinesAndArcs();
            breakArcsWithPoints();
            colorCrossovers();

            return MCamReturn.NoErrors;
        } 
    } 
}




            /*

            var upperCreaseID = new List<int>();
            var lowerCreaseID = new List<int>();
            var creaseX1 = 0.0;
            var creaseX2 = 0.0;
            var creaseY1 = 0.0;
            var creaseY2 = 0.0;
            var creaseX3 = 0.0;
            var creaseX4 = 0.0;
            var creaseY3 = 0.0;
            var creaseY4 = 0.0;

            void offsetCutchain()
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
                //Sets level 75 to main level and visible
                LevelsManager.SetMainLevel(75);
                LevelsManager.SetLevelVisible(75, true);
                LevelsManager.RefreshLevelsManager();
                GraphicsManager.Repaint(true);

                //Selects all geometry on level 75
                var selectedCutChain = ChainManager.ChainAll(75);
                //Creates and names level 500 and level 501
                int createdUpperLevel = 500;
                int createdLowerLevel = 501;
                LevelsManager.SetLevelName(500, "Upper Created Cut Geo");
                LevelsManager.SetLevelName(501, "Lower Created Cut Geo");

                //edits each entity of all chains
                foreach (var chain in selectedCutChain)
                {
                    //offsets line of lower
                    var lowerChainLarge = chain.OffsetChain2D(OffsetSideType.Left, .0225, OffsetRollCornerType.None, .5, false, .005, false);
                    var lowerChainSmall = chain.OffsetChain2D(OffsetSideType.Right, .0025, OffsetRollCornerType.None, .5, false, .005, false);
                    //Colors and selects result geometry
                    var cutResultGeometry = SearchManager.GetResultGeometry();
                    foreach (var entity in cutResultGeometry)
                    {
                        entity.Color = 11;
                        entity.Selected = true;
                        entity.Commit();
                    }
                    //Moves result geometry
                    GeometryManipulationManager.MoveSelectedGeometryToLevel(createdLowerLevel, true);
                    //Clears geometry in result
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));

                    //offsets line of upper
                    var upperChainLarge = chain.OffsetChain2D(OffsetSideType.Left, .0025, OffsetRollCornerType.None, .5, false, .005, false);
                    var upperChainSmall = chain.OffsetChain2D(OffsetSideType.Right, .0385, OffsetRollCornerType.None, .5, false, .005, false);
                    //Colors and selects result geometry
                    var cutResultGeometryNew = SearchManager.GetResultGeometry();
                    foreach (var entity in cutResultGeometryNew)
                    {
                        entity.Color = 10;
                        entity.Selected = true;
                        entity.Commit();
                    }
                    //Moves result geometry
                    GeometryManipulationManager.MoveSelectedGeometryToLevel(createdUpperLevel, true);
                    //Clears geometry in result
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
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
            }

            bool CreateLine2()
            {
                bool result = false;
                Point3D crease2 = new Point3D(creaseX2, creaseY2, 0.0);
                Point3D crease4 = new Point3D(creaseX4, creaseY4, 0.0);
                LineGeometry Line2 = new LineGeometry(crease2, crease4);
                Line2.Selected = true;
                result = Line2.Commit();
                return result;
            }

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
                GraphicsManager.Repaint(true);

                //Selects all geometry on level 101
                var selectedCreaseChain = ChainManager.ChainAll(101);
                //Creates and names level 502 and level 503
                int createdUpperCrease = 502;
                int createdLowerCrease = 503;
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
                        entity.Selected = true;
                        entity.Commit();
                    }
                    //Moves result geometry
                    GeometryManipulationManager.MoveSelectedGeometryToLevel(createdLowerCrease, true);
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
                        entity.Selected = true;
                        entity.Commit();
                    }
                    //Moves result geometry
                    GeometryManipulationManager.MoveSelectedGeometryToLevel(createdUpperCrease, true);
                    //Clears geometry in result
                    GraphicsManager.ClearColors(new GroupSelectionMask(true));
                }
            }

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
                            entity.Selected = true;
                            entity.Commit();
                        }
                        //Moves result geometry
                        int createdUpperCrease = 502;
                        GeometryManipulationManager.MoveSelectedGeometryToLevel(createdUpperCrease, true);
                        //Clears geometry in result
                        GraphicsManager.ClearColors(new GroupSelectionMask(true));
                        //Deselects all
                        SelectionManager.UnselectAllGeometry();

                        set = 0;
                    }
                }
            }

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
                            entity.Selected = true;
                            entity.Commit();
                        }
                        //Moves result geometry
                        int createdLowerCrease = 503;
                        GeometryManipulationManager.MoveSelectedGeometryToLevel(createdLowerCrease, true);
                        //Clears geometry in result
                        GraphicsManager.ClearColors(new GroupSelectionMask(true));
                        //Deselects all
                        SelectionManager.UnselectAllGeometry();

                        set = 0;
                    }
                }
            }


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
                GraphicsManager.Repaint(true);

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
                                line.Data.Point1.y += -1;
                                line.Data.Point2.y += +1;
                                line.Selected = false;
                            };
                            if (line.Data.Point1.y <= line.Data.Point2.y)
                            {
                                line.Data.Point1.y += +1;
                                line.Data.Point2.y += -1;
                                line.Selected = false;
                            };
                        };
                        //If line is horizontal
                        if (line.Data.Point1.y == line.Data.Point2.y)
                        {
                            if (line.Data.Point1.x >= line.Data.Point2.x)
                            {
                                line.Data.Point1.x += -1;
                                line.Data.Point2.x += +1;
                                line.Selected = false;
                            };
                            if (line.Data.Point1.x <= line.Data.Point2.x)
                            {
                                line.Data.Point1.x += +1;
                                line.Data.Point2.x += -1;
                                line.Selected = false;
                            };
                        };
                        //Stores result in Mastercam
                        line.Commit();
                    }
                    //Updates screen shown
                    GraphicsManager.Repaint(true);
                }
            }

            DemoAlterLine();
            offsetCreasechain();
            connectUpperLines();
            connectLowerLines();
            offsetCutchain();



            return MCamReturn.NoErrors;
        }
    }
}

            */

                



            

        


    


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
