﻿using System;
using System.Diagnostics;
using cpArbiter = System.IntPtr;
using cpBool = System.Byte;
using cpCollisionHandlerPointer = System.IntPtr;
using cpSpace = System.IntPtr;
using voidptr_t = System.IntPtr;

#if __IOS__ || __TVOS__ || __WATCHOS__ || __MACCATALYST__
using ObjCRuntime;
#endif

namespace ChipmunkBinding
{
    /// <summary>
    /// A collision handler is a set of 4 function callbacks for the different collision events that
    /// Chipmunk recognizes. Collision callbacks are closely associated with <see cref="Arbiter"/>
    /// objects. You should familiarize yourself with those as well. Note #1: Shapes tagged as
    /// sensors (<see cref="Shape.Sensor"/> == true) never generate collisions that get processed,
    /// so collisions between sensor shapes and other shapes will never call the post_solve()
    /// callback. They still generate begin() and separate() callbacks, and the pre_solve() callback
    /// is also called every frame even though there is no collision response. Note #2: pre_solve()
    /// callbacks are called before the sleeping algorithm runs. If an object falls asleep, its
    /// post_solve() callback won’t be called until it’s re-awoken.
    /// </summary>
    public sealed class CollisionHandler
    {
        private readonly cpCollisionHandlerPointer handle;

        private static CollisionBeginFunction beginCallback = CollisionBeginFunctionCallback;
        private static CollisionPreSolveFunction preSolveCallback = CollisionPreSolveFunctionCallback;
        private static CollisionPostSolveFunction postSolveCallback = CollisionPostSolveFunctionCallback;
        private static CollisionSeparateFunction separeteCallback = CollisionSeparateFunctionCallback;

        private static IntPtr DefaultBeginFunction;
        private static IntPtr DefaultPreSolveFunction;
        private static IntPtr DefaultPostSolveFunction;
        private static IntPtr DefaultSeparateFunction;

        private CollisionHandler(cpCollisionHandlerPointer collisionHandle, ref cpCollisionHandler handler)
        {
            handle = collisionHandle;

            IntPtr data = NativeInterop.RegisterHandle(this);

            handler.userData = data;

            long typeA = (long)handler.typeA.ToUInt64();
            long typeB = (long)handler.typeB.ToUInt64();

            TypeA = unchecked((int)typeA);
            TypeB = unchecked((int)typeB);

            cpCollisionHandler.ToPointer(handler, handle);
        }

        internal static CollisionHandler GetOrCreate(cpCollisionHandlerPointer collisionHandle)
        {
            Debug.Assert(collisionHandle != IntPtr.Zero, "CollisionHandle cannot be zero");

            var handler = cpCollisionHandler.FromHandle(collisionHandle);

            if (handler.userData != IntPtr.Zero)
            {
                return NativeInterop.FromIntPtr<CollisionHandler>(handler.userData);
            }

            EnsureDefaultCallbackValues(handler);

            return new CollisionHandler(collisionHandle, ref handler);
        }

        private static void EnsureDefaultCallbackValues(cpCollisionHandler handler)
        {
            if (DefaultBeginFunction != IntPtr.Zero)
                return;

            DefaultBeginFunction = handler.beginFunction;
            DefaultPreSolveFunction = handler.preSolveFunction;
            DefaultPostSolveFunction = handler.postSolveFunction;
            DefaultSeparateFunction = handler.separateFunction;
        }

        private Action<Arbiter, Space, object> begin;
        /// <summary>
        /// This function is called when two shapes with types that match this collision handler begin colliding
        /// </summary>
        public Action<Arbiter, Space, object> Begin
        {
            set
            {
                begin = value;

                var handler = cpCollisionHandler.FromHandle(handle);

                IntPtr callbackPointer;

                if (value == null)
                {
                    callbackPointer = DefaultBeginFunction;
                }
                else
                {
                    callbackPointer = beginCallback.ToFunctionPointer();
                }

                handler.beginFunction = callbackPointer;

                cpCollisionHandler.ToPointer(handler, handle);
            }
            get => begin;
        }

        private Func<Arbiter, Space, object, bool> preSolve;

        /// <summary>
        /// This function is called each step when two shapes with types that match this collision
        /// handler are colliding. It's called before the collision solver runs so that you can
        /// affect a collision's outcome.
        /// </summary>
        public Func<Arbiter, Space, object, bool> PreSolve
        {
            set
            {
                preSolve = value;
                var handler = cpCollisionHandler.FromHandle(handle);
                IntPtr callbackPointer;

                if (value == null)
                {
                    callbackPointer = DefaultPreSolveFunction;
                }
                else
                {
                    callbackPointer = preSolveCallback.ToFunctionPointer();
                }

                handler.preSolveFunction = callbackPointer;
                cpCollisionHandler.ToPointer(handler, handle);
            }
            get => preSolve;
        }

        private Action<Arbiter, Space, object> postSolve;

        /// <summary>
        /// This function is called each step when two shapes with types that match this collision
        /// handler are colliding. It's called after the collision solver runs so that you can read
        /// back information about the collision to trigger events in your game.
        /// </summary>
        public Action<Arbiter, Space, object> PostSolve
        {
            set
            {
                postSolve = value;
                var handler = cpCollisionHandler.FromHandle(handle);
                IntPtr callbackPointer;

                if (value == null)
                {
                    callbackPointer = DefaultPostSolveFunction;
                }
                else
                {
                    callbackPointer = postSolveCallback.ToFunctionPointer();
                }

                handler.postSolveFunction = callbackPointer;
                cpCollisionHandler.ToPointer(handler, handle);
            }
            get => postSolve;
        }

        private Action<Arbiter, Space, object> separate;

        /// <summary>
        /// This function is called when two shapes with types that match this collision handler
        /// stop colliding.
        /// </summary>
        public Action<Arbiter, Space, object> Separate
        {
            set
            {
                separate = value;
                var handler = cpCollisionHandler.FromHandle(handle);
                IntPtr callbackPointer;

                if (value == null)
                {
                    callbackPointer = DefaultSeparateFunction;
                }
                else
                {
                    callbackPointer = separeteCallback.ToFunctionPointer();
                }

                handler.separateFunction = callbackPointer;
                cpCollisionHandler.ToPointer(handler, handle);
            }
            get => separate;
        }

        /// <summary>
        /// User definable context pointer that is passed to all of the collision handler functions.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// In the collision handler callback, the shape with this type will be the first argument.
        /// Read only.
        /// </summary>
        public int TypeA { get; }

        /// <summary>
        /// In the collision handler callback, the shape with this type will be the second argument.
        /// Read only.
        /// </summary>
        public int TypeB { get; }

#if __IOS__ || __TVOS__ || __WATCHOS__ || __MACCATALYST__
        [MonoPInvokeCallback(typeof(CollisionBeginFunction))]
#endif
        private static void CollisionBeginFunctionCallback(cpArbiter arbiterHandle, cpSpace spaceHandle, voidptr_t userData)
        {
            var arbiter = new Arbiter(arbiterHandle);
            var space = Space.FromHandle(spaceHandle);

            var handler = NativeInterop.FromIntPtr<CollisionHandler>(userData);
            var begin = handler.Begin;

            if (begin == null)
            {
                return;
            }

            begin(arbiter, space, handler.Data);
        }

#if __IOS__ || __TVOS__ || __WATCHOS__ || __MACCATALYST__
        [MonoPInvokeCallback(typeof(CollisionPreSolveFunction))]
#endif
        private static cpBool CollisionPreSolveFunctionCallback(cpArbiter arbiterHandle, cpSpace spaceHandle, voidptr_t userData)
        {
            var arbiter = new Arbiter(arbiterHandle);
            var space = Space.FromHandle(spaceHandle);

            var handler = NativeInterop.FromIntPtr<CollisionHandler>(userData);
            var preSolve = handler.PreSolve;

            if (preSolve == null)
            {
                return 1;
            }

            if (preSolve(arbiter, space, handler.Data))
            {
                return 1;
            }

            return 0;
        }

#if __IOS__ || __TVOS__ || __WATCHOS__ || __MACCATALYST__
        [MonoPInvokeCallback(typeof(CollisionPostSolveFunction))]
#endif
        private static void CollisionPostSolveFunctionCallback(cpArbiter arbiterHandle, cpSpace spaceHandle, voidptr_t userData)
        {
            var arbiter = new Arbiter(arbiterHandle);
            var space = Space.FromHandle(spaceHandle);

            var handler = NativeInterop.FromIntPtr<CollisionHandler>(userData);
            var postSolve = handler.PostSolve;

            if (postSolve == null)
            {
                return;
            }

            postSolve(arbiter, space, handler.Data);
        }

#if __IOS__ || __TVOS__ || __WATCHOS__ || __MACCATALYST__
        [MonoPInvokeCallback(typeof(CollisionSeparateFunction))]
#endif
        private static void CollisionSeparateFunctionCallback(cpArbiter arbiterHandle, cpSpace spaceHandle, voidptr_t userData)
        {
            var arbiter = new Arbiter(arbiterHandle);
            var space = Space.FromHandle(spaceHandle);

            var handler = NativeInterop.FromIntPtr<CollisionHandler>(userData);
            var separate = handler.Separate;

            if (separate == null)
            {
                return;
            }

            separate(arbiter, space, handler.Data);
        }
    }
}
