﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SharpGLTF.Schema2
{
    using Collections;

    [System.Diagnostics.DebuggerDisplay("Mesh[{LogicalIndex}] {Name}")]
    public sealed partial class Mesh
    {
        #region lifecycle

        internal Mesh()
        {
            _primitives = new ChildrenCollection<MeshPrimitive, Mesh>(this);
            _weights = new List<double>();
        }

        #endregion

        #region properties

        public int LogicalIndex => this.LogicalParent.LogicalMeshes.IndexOfReference(this);

        public IEnumerable<Node> VisualParents => Node.GetNodesUsingMesh(this);

        public IReadOnlyList<MeshPrimitive> Primitives => _primitives;

        public IReadOnlyList<Single> MorphWeights => _weights.Select(item => (Single)item).ToArray();

        public Transforms.BoundingBox3? LocalBounds3 => Transforms.BoundingBox3.UnionOf(Primitives.Select(item => item.LocalBounds3));

        #endregion

        #region API

        public MeshPrimitive CreatePrimitive()
        {
            var mp = new MeshPrimitive();

            _primitives.Add(mp);

            return mp;
        }

        public override IEnumerable<Exception> Validate()
        {
            var exx = base.Validate().ToList();

            foreach (var p in this.Primitives)
            {
                exx.AddRange(p.Validate());
            }

            return exx;
        }

        #endregion
    }

    public partial class ModelRoot
    {
        public Mesh CreateMesh(string name = null)
        {
            var dstMesh = new Mesh();
            dstMesh.Name = name;

            this._meshes.Add(dstMesh);

            return dstMesh;
        }
    }
}