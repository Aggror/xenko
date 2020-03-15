// Copyright (c) Xenko contributors (https://xenko.com) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Xenko.Core;
using Xenko.Core.Mathematics;
using Xenko.Engine.Design;
using Xenko.Engine.Processors;
using Xenko.Engine.Spline;

namespace Xenko.Engine
{
    /// <summary>
    /// Component representing a  Spline node.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Associate this component to an entity to maintain bezier curves that create a spline.
    /// </para>
    /// </remarks>

    [DataContract]
    [DefaultEntityComponentProcessor(typeof(SplineNodeTransformProcessor), ExecutionMode = ExecutionMode.All)]
    [Display("spline node")]
    [ComponentCategory("Splines")]
    public sealed class SplineNodeComponent : EntityComponent
    {
        public SplineNodeComponent Next { get; set; }
        public SplineNodeComponent Previous { get; set; }

        public Vector3 OutHandler { get; set; }
        public Vector3 InHandler { get; set; }

        private int _segments = 2;
        public int Segments
        {
            get { return _segments; }
            set
            {
                if (value < 2)
                {
                    _segments = 2;
                }
                else
                {
                    _segments = value;
                }
            }
        }

        public bool DebugSegments { get; set; }
        public bool DebugNodesLink { get; set; }
        public bool DebugOutHandler { get; set; }
        public bool IsDirty { get; set; }

        private SplineNode _splineNode;
        private Vector3 _previousVector;

        internal void Update(TransformComponent transformComponent)
        {
            CheckDirtyness();

            if (Next != null && IsDirty)
            {
                UpdateBezierCurve();
            }

            if (Previous != null && IsDirty)
            {
                Previous.UpdateBezierCurve();
            }


            _previousVector = Entity.Transform.Position;
        }

        private void CheckDirtyness()
        {
            if (_previousVector.X != Entity.Transform.Position.X ||
                            _previousVector.Y != Entity.Transform.Position.Y ||
                            _previousVector.Z != Entity.Transform.Position.Z)
            {
                IsDirty = true;
            }
        }

        public void UpdateBezierCurve()
        {
            Vector3 scale;
            Quaternion rotation;
            Vector3 translation;
            Vector3 translation2;
            Entity.Transform.WorldMatrix.Decompose(out scale, out rotation, out translation);
            Next.Entity.Transform.WorldMatrix.Decompose(out scale, out rotation, out translation2);
            _splineNode = new SplineNode(Segments, translation, OutHandler, translation2, Next.InHandler);
        }

        public SplineNode GetSplineNode()
        {
            if (_splineNode == null)
            {
                UpdateBezierCurve();
            }

            return _splineNode;
        }
    }
}
