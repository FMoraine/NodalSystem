using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Nodal.GUIConnector;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Settings;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using NodalInteractiveCreator.Objects.Puzzle;
using UnityEditor;
using UnityEngine;
using PuzzleMath = Machinika.Objects.Puzzle.PuzzleMath;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Editor.NodalSystem.Nodal
{
    public class BezierLink
    {
        public EmitterConnector emit;
        public ReceiverConnector receive;

        protected BezierCurve bezier = new BezierCurve();

        public Action<BezierLink> OnDelete;

        public NodalLayer layerEmit;
        public NodalLayer layerReceiv;
        public Color colorBezier;

        public BezierLink(EmitterConnector boundsEmitter, ReceiverConnector boundsReceiver, NodalLayer layerEmit , NodalLayer layerReceiv)
        {
            receive = boundsReceiver;
            emit = boundsEmitter;
            this.layerEmit = layerEmit;
            this.layerReceiv = layerReceiv;
            colorBezier = NICStettings.Settings.preferences.colorLink;
        }

        public void Draw()
        {
            DrawNodeCurve(emit.GetLayerBound(layerEmit), receive.GetLayerBound(layerReceiv));

            Vector3 pos = PuzzleMath.Casteljau(bezier.point1 , bezier.point2 , bezier.curvePoint1 , bezier.curvePoint2 , 0.5f);
            if (GUI.Button(new Rect(pos.x, pos.y, 10, 10), new GUIContent("X", "Delete")) && OnDelete != null)
            {
                OnDelete.Invoke(this);
            }
        }

        void DrawNodeCurve(Rect start, Rect end)
        {
    
            bezier.point1 = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
            bezier.point2 = new Vector3(end.x, end.y + end.height / 2, 0);
            bezier.curvePoint1 = bezier.point1 + Vector3.right * 50;
            bezier.curvePoint2 = bezier.point2 + Vector3.left * 50;
            
            Handles.DrawBezier(bezier.point1, bezier.point2, bezier.curvePoint1, bezier.curvePoint2, colorBezier, null, 4);
        }
    }
}
