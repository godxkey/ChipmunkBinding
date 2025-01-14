﻿namespace ChipmunkBinding
{
    /// <summary>
    /// <see cref="PinJoint"/> links shapes with a solid bar or pin. Keeps the anchor points at a
    /// set distance from one another.
    /// </summary>
    public class PinJoint : Constraint
    {
        /// <summary>
        /// Check if a constraint is a <see cref="PinJoint"/>.
        /// </summary>
        public static bool IsPinJoint(Constraint constraint) => NativeMethods.cpConstraintIsPinJoint(constraint.Handle) != 0;

        /// <summary>
        /// The distance between the two anchor points is measured when the joint is created. If you
        /// want to set a specific distance, use the setter function to override it.
        /// </summary>
        /// <param name="bodyA">One of the two bodies to connect.</param>
        /// <param name="bodyB">One of the two bodies to connect.</param>
        /// <param name="anchorA">The anchor point for <paramref name="bodyA"/>.</param>
        /// <param name="anchorB">The anchor point for <paramref name="bodyB"/>.</param>
        public PinJoint(Body bodyA, Body bodyB, Vect anchorA, Vect anchorB)
            : base(NativeMethods.cpPinJointNew(bodyA.Handle, bodyB.Handle, anchorA, anchorB))
        {
        }

        /// <summary>
        /// The location of the first anchor relative to the first body.
        /// </summary>
        public Vect AnchorA
        {
            get => NativeMethods.cpPinJointGetAnchorA(Handle);
            set => NativeMethods.cpPinJointSetAnchorA(Handle, value);
        }

        /// <summary>
        /// The location of the second anchor relative to the second body.
        /// </summary>
        public Vect AnchorB
        {
            get => NativeMethods.cpPinJointGetAnchorB(Handle);
            set => NativeMethods.cpPinJointSetAnchorB(Handle, value);
        }

        /// <summary>
        /// The distance the joint will maintain between the two anchors.
        /// </summary>
        public double Distance
        {
            get => NativeMethods.cpPinJointGetDist(Handle);
            set => NativeMethods.cpPinJointSetDist(Handle, value);
        }
    }
}
