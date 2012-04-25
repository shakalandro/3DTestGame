using System;
using System.Collections.Generic;
using System.Text;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JiggleGame.PhysicObjects
{
    public class RagdollObject : PhysicObject
    {

        public enum RagdollType
        {
            Simple,Complex
        }

        private enum LimbId
        {
            Torso,
            Head,
            UpperLegLeft,
            UpperLegRight,
            LowerLegLeft,
            LowerLegRight,
            UpperArmLeft,
            UpperArmRight,
            LowerArmLeft,
            LowerArmRight,
            FootLeft,
            FootRight,
            HandLeft,
            HandRight,
            Hips,
            NumLimbs
        }

        private enum JointId
        {
            Neck,
            ShoulderLeft,
            ShoulderRight,
            ElbowLeft,
            ElbowRight,
            HipLeft,
            HipRight,
            KneeLeft,
            KneeRight,
            WristLeft,
            WristRight,
            AnkleLeft,
            AnkleRight,
            Spine,
            NumJoints
        }

        PhysicObject[] limbs;
        HingeJoint[] joints;

        int numLimbs;
        int numJoints;

        private void DisableCollisions(Body rb0, Body rb1)
        {
            if ((rb0.CollisionSkin == null) || (rb1.CollisionSkin == null))
                return;
            rb0.CollisionSkin.NonCollidables.Add(rb1.CollisionSkin);
            rb1.CollisionSkin.NonCollidables.Add(rb0.CollisionSkin);
        }

        public RagdollObject(Game game,Model capsule,Model sphere,Model box, RagdollType type,float density)
            : base(game)
        {
            if (type == RagdollType.Complex)
            {
                numLimbs = (int)LimbId.NumLimbs;
                numJoints = (int)JointId.NumJoints;
            }
            else
            {
                numLimbs = (int)LimbId.NumLimbs - 5;
                numJoints = (int)JointId.NumJoints - 5;
            }

            limbs = new PhysicObject[numLimbs];
            joints = new HingeJoint[numJoints];

            limbs[(int)LimbId.Head] = new SphereObject(this.Game, sphere, 0.15f, Matrix.Identity, Vector3.Zero);
            limbs[(int)LimbId.UpperLegLeft] = new CapsuleObject(this.Game, capsule, 0.08f, 0.3f, Matrix.CreateRotationX(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.UpperLegRight] = new CapsuleObject(this.Game, capsule, 0.08f, 0.3f, Matrix.CreateRotationX(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.LowerLegLeft] = new CapsuleObject(this.Game, capsule, 0.08f, 0.3f, Matrix.CreateRotationX(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.LowerLegRight] = new CapsuleObject(this.Game, capsule, 0.08f, 0.3f, Matrix.CreateRotationX(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.UpperArmLeft] = new CapsuleObject(this.Game, capsule, 0.07f, 0.2f, Matrix.CreateRotationZ(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.UpperArmRight] = new CapsuleObject(this.Game, capsule, 0.07f, 0.2f, Matrix.CreateRotationZ(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.LowerArmLeft] = new CapsuleObject(this.Game, capsule, 0.06f, 0.2f, Matrix.CreateRotationZ(MathHelper.ToRadians(90)), Vector3.Zero);
            limbs[(int)LimbId.LowerArmRight] = new CapsuleObject(this.Game, capsule, 0.06f, 0.2f, Matrix.CreateRotationZ(MathHelper.ToRadians(90)), Vector3.Zero);

            if (type == RagdollType.Complex)
            {
                limbs[(int)LimbId.FootLeft] = new SphereObject(this.Game, sphere, 0.07f, Matrix.Identity, Vector3.Zero);
                limbs[(int)LimbId.FootRight] = new SphereObject(this.Game, sphere, 0.07f, Matrix.Identity, Vector3.Zero);
                limbs[(int)LimbId.HandLeft] = new SphereObject(this.Game, sphere, 0.05f, Matrix.Identity, Vector3.Zero);
                limbs[(int)LimbId.HandRight] = new SphereObject(this.Game, sphere, 0.05f, Matrix.Identity, Vector3.Zero);
                limbs[(int)LimbId.Torso] = new BoxObject(this.Game, box, new Vector3(0.2f, 0.4f,0.35f), Matrix.Identity, Vector3.Zero);
                limbs[(int)LimbId.Hips] = new BoxObject(this.Game, box, new Vector3(0.2f, 0.2f,0.35f), Matrix.Identity, Vector3.Zero);
            }
            else
            {
                limbs[(int)LimbId.Torso] = new BoxObject(this.Game, box, new Vector3(0.2f, 0.6f, 0.35f), Matrix.Identity, Vector3.Zero);
            }

            limbs[(int)LimbId.Head].PhysicsBody.Position = new Vector3(0.03f, 0.5f,0);
            limbs[(int)LimbId.UpperLegLeft].PhysicsBody.Position = new Vector3(0, -0.4f, 0.12f);
            limbs[(int)LimbId.UpperLegRight].PhysicsBody.Position = new Vector3(0, -0.4f, -0.12f);
            limbs[(int)LimbId.LowerLegLeft].PhysicsBody.Position = new Vector3(0, -0.7f, 0.12f);
            limbs[(int)LimbId.LowerLegRight].PhysicsBody.Position = new Vector3(0, -0.7f, -0.12f);
            limbs[(int)LimbId.UpperArmLeft].PhysicsBody.Position = new Vector3(0, 0.25f, 0.25f);
            limbs[(int)LimbId.UpperArmRight].PhysicsBody.Position = new Vector3(0, 0.25f, -0.25f);
            limbs[(int)LimbId.LowerArmLeft].PhysicsBody.Position = new Vector3(0, 0.25f, 0.5f);
            limbs[(int)LimbId.LowerArmRight].PhysicsBody.Position = new Vector3(0, 0.25f, -0.5f);

            if (type == RagdollType.Complex)
            {
                limbs[(int)LimbId.FootLeft].PhysicsBody.Position = new Vector3(0.13f, -0.85f, 0.12f);
                limbs[(int)LimbId.FootRight].PhysicsBody.Position = new Vector3(0.13f, -0.85f, -0.12f);
                limbs[(int)LimbId.HandLeft].PhysicsBody.Position = new Vector3(0, 0.25f, 0.72f);
                limbs[(int)LimbId.HandRight].PhysicsBody.Position = new Vector3(0, 0.25f, -0.72f);
                limbs[(int)LimbId.Torso].PhysicsBody.Position = new Vector3(0, 0.2f, 0.0f);
                limbs[(int)LimbId.Hips].PhysicsBody.Position = new Vector3(0, -0.1f, 0.0f);
            }
            else
            {
                limbs[(int)LimbId.Torso].PhysicsBody.Position = new Vector3(0, 0, 0);
            }


            // set up hinge joints
            float haldWidth = 0.2f;
            float sidewaysSlack = 0.1f;
            float damping = 0.5f;

            for (int i = 0; i < numJoints; i++)
                joints[i] = new HingeJoint();

            if (type == RagdollType.Complex)
            {
                joints[(int)JointId.Spine].Initialise(limbs[(int)LimbId.Hips].PhysicsBody,
                                        limbs[(int)LimbId.Torso].PhysicsBody,
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, 0.1f, 0.0f),
                                        haldWidth, 70.0f, 30.0f,
                                        3.0f * sidewaysSlack,
                                        damping);

                joints[(int)JointId.Neck].Initialise(limbs[(int)LimbId.Hips].PhysicsBody,
                                        limbs[(int)LimbId.Head].PhysicsBody,
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(-0.05f, 0.25f, 0.0f),
                                        haldWidth, 50.0f, 20.0f,
                                        3.0f * sidewaysSlack,
                                        damping);

                joints[(int)JointId.ShoulderLeft].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                                        limbs[(int)LimbId.UpperArmLeft].PhysicsBody,
                                        new Vector3(1.0f, 0.0f, 0.0f),
                                        new Vector3(0.0f, 0.05f, 0.15f),
                                        haldWidth, 70.0f, 30.0f,
                                        0.7f,
                                        damping);

                joints[(int)JointId.ShoulderRight].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                                        limbs[(int)LimbId.UpperArmRight].PhysicsBody,
                                        new Vector3(1.0f, 0.0f, 0.0f),
                                        new Vector3(0.0f, 0.05f, -0.15f),
                                        haldWidth, 30.0f, 75.0f,
                                        0.7f,
                                        damping);

                joints[(int)JointId.HipLeft].Initialise(limbs[(int)LimbId.Hips].PhysicsBody,
                                        limbs[(int)LimbId.UpperLegLeft].PhysicsBody,
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, -0.25f, 0.12f),
                                        haldWidth, 10.0f, 60.0f,
                                        0.4f,
                                        damping);

                joints[(int)JointId.HipRight].Initialise(limbs[(int)LimbId.Hips].PhysicsBody,
                                        limbs[(int)LimbId.UpperLegRight].PhysicsBody,
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, -0.25f, -0.12f),
                                        haldWidth, 10.0f, 60.0f,
                                        0.4f,
                                        damping);

                joints[(int)JointId.AnkleLeft].Initialise(limbs[(int)LimbId.LowerLegLeft].PhysicsBody,
                                        limbs[(int)LimbId.FootLeft].PhysicsBody,
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, -0.15f, 0.15f),
                                        haldWidth, 30.0f, 10.0f,
                                        0.01f,
                                        damping);

                joints[(int)JointId.AnkleRight].Initialise(limbs[(int)LimbId.LowerLegRight].PhysicsBody,
                                        limbs[(int)LimbId.FootRight].PhysicsBody,
                                        new Vector3(0.0f, 0.0f, 1.0f),
                                        new Vector3(0.0f, -0.15f, -0.15f),
                                        haldWidth, 30.0f, 10.0f,
                                        0.01f,
                                         damping);

                joints[(int)JointId.WristLeft].Initialise(limbs[(int)LimbId.LowerArmLeft].PhysicsBody,
                                        limbs[(int)LimbId.HandLeft].PhysicsBody,
                                        new Vector3(0.0f, 1.0f, 0.0f),
                                        new Vector3(0.0f, 0.0f, 0.12f),
                                        haldWidth, 45.0f, 70.0f,
                                        0.01f,
                                        damping);

                joints[(int)JointId.WristRight].Initialise(limbs[(int)LimbId.LowerArmRight].PhysicsBody,
                                        limbs[(int)LimbId.HandRight].PhysicsBody,
                                        new Vector3(0.0f, 1.0f, 0.0f),
                                        new Vector3(0.0f, 0.0f, -0.12f),
                                        haldWidth, 45.0f, 70.0f,
                                        0.01f,
                                        damping);

            }
            else
            {
                joints[(int)JointId.Neck].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                              limbs[(int)LimbId.Head].PhysicsBody,
                              new Vector3(0.0f, 0.0f, 1.0f),
                              new Vector3(-0.05f, 0.25f, 0.0f),
                              haldWidth, 50.0f, 20.0f,
                              3 * sidewaysSlack,
                              damping);

                joints[(int)JointId.ShoulderLeft].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                              limbs[(int)LimbId.UpperArmLeft].PhysicsBody,
                              new Vector3(1.0f, 0.0f, 0.0f),
                              new Vector3(0.0f, 0.25f, 0.15f),
                              haldWidth, 30.0f, 75.0f,
                              0.7f,
                              damping);

                joints[(int)JointId.ShoulderRight].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                              limbs[(int)LimbId.UpperArmRight].PhysicsBody,
                              new Vector3(1.0f, 0.0f, 0.0f),
                              new Vector3(0.0f, 0.25f, -0.15f),
                              haldWidth, 75.0f, 30.0f,
                              0.7f,
                              damping);


                joints[(int)JointId.HipLeft].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                              limbs[(int)LimbId.UpperLegLeft].PhysicsBody,
                              new Vector3(0.0f, 0.0f, 1.0f),
                              new Vector3(0.0f, -0.25f, 0.12f),
                              haldWidth, 10.0f, 60.0f,
                              0.4f,
                              damping);


                joints[(int)JointId.HipRight].Initialise(limbs[(int)LimbId.Torso].PhysicsBody,
                              limbs[(int)LimbId.UpperLegRight].PhysicsBody,
                              new Vector3(0.0f, 0.0f, 1.0f),
                              new Vector3(0.0f, -0.25f, -0.12f),
                              haldWidth, 10.0f, 60.0f,
                              0.4f,
                              damping);


            }


            joints[(int)JointId.KneeLeft].Initialise(limbs[(int)LimbId.UpperLegLeft].PhysicsBody,
                        limbs[(int)LimbId.LowerLegLeft].PhysicsBody,
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, -0.15f, 0.0f),
                        haldWidth, 100.0f, 0.0f,
                        sidewaysSlack,
                        damping);

            joints[(int)JointId.KneeRight].Initialise(limbs[(int)LimbId.UpperLegRight].PhysicsBody,
                        limbs[(int)LimbId.LowerLegRight].PhysicsBody,
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, -0.15f, 0.0f),
                        haldWidth, 100.0f, 0.0f,
                        sidewaysSlack,
                        damping);

            joints[(int)JointId.ElbowLeft].Initialise(limbs[(int)LimbId.UpperArmLeft].PhysicsBody,
                        limbs[(int)LimbId.LowerArmLeft].PhysicsBody,
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, 0.0f, 0.13f),
                        haldWidth, 0.0f, 130.0f,
                        sidewaysSlack,
                        damping);

            joints[(int)JointId.ElbowRight].Initialise(limbs[(int)LimbId.UpperArmRight].PhysicsBody,
                        limbs[(int)LimbId.LowerArmRight].PhysicsBody,
                        new Vector3(0.0f, 0.0f, 1.0f),
                        new Vector3(0.0f, 0.0f, -0.13f),
                        haldWidth, 130.0f, 0.0f,
                        sidewaysSlack,
                        damping);



            // disable some collisions between adjacent pairs
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.UpperLegLeft].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.UpperLegRight].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.UpperArmLeft].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.UpperArmRight].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperLegLeft].PhysicsBody, limbs[(int)LimbId.LowerLegLeft].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperLegRight].PhysicsBody, limbs[(int)LimbId.LowerLegRight].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperArmLeft].PhysicsBody, limbs[(int)LimbId.LowerArmLeft].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperArmRight].PhysicsBody, limbs[(int)LimbId.LowerArmRight].PhysicsBody);

            if (type == RagdollType.Complex)
            {
                DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.Hips].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.LowerLegLeft].PhysicsBody, limbs[(int)LimbId.FootLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.LowerLegRight].PhysicsBody, limbs[(int)LimbId.FootRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.LowerArmLeft].PhysicsBody, limbs[(int)LimbId.HandLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.LowerArmRight].PhysicsBody, limbs[(int)LimbId.HandRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.UpperLegLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.UpperLegRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.UpperArmLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.UpperArmRight].PhysicsBody);
            }

            // he's not double-jointed...
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.LowerLegLeft].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.LowerLegRight].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperArmLeft].PhysicsBody, limbs[(int)LimbId.UpperArmRight].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperArmRight].PhysicsBody, limbs[(int)LimbId.UpperArmLeft].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperArmLeft].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperArmRight].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperLegLeft].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.UpperLegRight].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.LowerLegLeft].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.LowerLegRight].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.LowerLegLeft].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
            DisableCollisions(limbs[(int)LimbId.LowerLegRight].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);

            if (type == RagdollType.Complex)
            {
                DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.FootLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Torso].PhysicsBody, limbs[(int)LimbId.FootRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.FootLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.FootRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.LowerLegLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.LowerLegRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.LowerArmLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.Hips].PhysicsBody, limbs[(int)LimbId.LowerArmRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootLeft].PhysicsBody, limbs[(int)LimbId.HandLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootRight].PhysicsBody, limbs[(int)LimbId.HandRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootLeft].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootRight].PhysicsBody, limbs[(int)LimbId.Head].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootLeft].PhysicsBody, limbs[(int)LimbId.UpperArmLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootRight].PhysicsBody, limbs[(int)LimbId.UpperArmRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootLeft].PhysicsBody, limbs[(int)LimbId.UpperArmRight].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootRight].PhysicsBody, limbs[(int)LimbId.UpperArmLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootLeft].PhysicsBody, limbs[(int)LimbId.UpperLegLeft].PhysicsBody);
                DisableCollisions(limbs[(int)LimbId.FootRight].PhysicsBody, limbs[(int)LimbId.UpperLegRight].PhysicsBody);
            }

            foreach (HingeJoint joint in joints)
            {
                if (joint != null)
                    joint.EnableHinge();
            }

            foreach (PhysicObject limb in limbs)
            {
                if (limb != null)
                {
                    limb.PhysicsBody.CollisionSkin.SetMaterialProperties(0, new JigLibX.Collision.MaterialProperties(0.2f, 3.0f, 2.0f));
                    this.Game.Components.Add(limb);
                }
            }
           
        }

        public void PutToSleep()
        {
            foreach (PhysicObject limb in limbs)
            {
                if (limb != null)
                    limb.PhysicsBody.SetInactive();
            } 
        }

        private void MoveTorso(Vector3 pos)
        {
            Vector3 delta = pos - limbs[(int)LimbId.Torso].PhysicsBody.Position;
            foreach (PhysicObject limb in limbs)
            {
                if (limb != null)
                {
                    Vector3 origPos = limb.PhysicsBody.Position;
                    limb.PhysicsBody.MoveTo(origPos + delta, limb.PhysicsBody.Orientation);
                }
            }
        }

        public Vector3 Position
        {
            set { MoveTorso(value); }
            get { return limbs[(int)LimbId.Torso].PhysicsBody.Position; }
        }


        public override void ApplyEffects(Microsoft.Xna.Framework.Graphics.BasicEffect effect)
        {
            //
        }
    }
}
