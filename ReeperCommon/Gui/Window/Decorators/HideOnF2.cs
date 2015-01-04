﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReeperCommon.Gui.Window.Decorators
{
    public class HideOnF2 : WindowDecorator
    {
        private bool _state = true;

        public HideOnF2(IWindowComponent baseComponent) : base(baseComponent)
        {
            GameEvents.onShowUI.Add(Show);
            GameEvents.onHideUI.Add(Hide);
        }

        ~HideOnF2()
        {
            GameEvents.onHideUI.Remove(Hide);
            GameEvents.onShowUI.Remove(Show);
        }

        private void Show()
        {
            _state = true;
        }

        private void Hide()
        {
            _state = false;
        }

        public override void OnPreWindowDraw()
        {
            base.OnPreWindowDraw();
            Visible = _state; // update every frame in case of external change
        }
    }
}
