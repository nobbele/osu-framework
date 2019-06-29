﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Android.Views;
using osu.Framework.Input.Handlers;
using osu.Framework.Input.StateChanges;
using osu.Framework.Platform;
using osuTK.Input;

namespace osu.Framework.Android.Input
{
    public class AndroidTouchHandler : InputHandler
    {
        private readonly AndroidGameView view;

        public AndroidTouchHandler(AndroidGameView view)
        {
            this.view = view;
            view.Touch += handleTouches;
        }

        private void handleTouches(object sender, View.TouchEventArgs e)
        {
            int ptrId = e.Event.GetPointerId(e.Event.ActionIndex);
            MouseButton button = ptrId == 0 ? MouseButton.Left : MouseButton.Right;

            if (button == MouseButton.Left)
            {
                PendingInputs.Enqueue(new MousePositionAbsoluteInput
                {
                    Position = new osuTK.Vector2(e.Event.GetX() * view.ScaleX, e.Event.GetY() * view.ScaleY)
                });
            }

            switch (e.Event.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Move:
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    PendingInputs.Enqueue(new MouseButtonInput(button, true));
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    PendingInputs.Enqueue(new MouseButtonInput(button, false));
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            view.Touch -= handleTouches;
            base.Dispose(disposing);
        }

        public override bool IsActive => true;

        public override int Priority => 0;

        public override bool Initialize(GameHost host) => true;
    }
}
