/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * ironpy@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Project;

namespace Microsoft.IronStudio.Project {
    /// <summary>
    /// Base class for property pages based on a WinForm control.
    /// </summary>
    public abstract class CommonPropertyPage : IPropertyPage {
        private IPropertyPageSite _site;
        private bool _dirty;
        private ProjectNode _project;

        public abstract Control Control {
            get;
        }

        public abstract void Apply();
        public abstract void LoadSettings();

        public abstract string Name {
            get;
        }

        public ProjectNode ProjectMgr {
            get {
                return _project;
            }
        }

        
        public bool IsDirty {
            get {
                return _dirty;
            }
            set {
                if (_dirty != value) {
                    _dirty = value;
                    if (_site != null) {
                        _site.OnStatusChange((uint)(_dirty ? PropPageStatus.Dirty : PropPageStatus.Clean));
                    }
                }
            }
        }

        void IPropertyPage.Activate(IntPtr hWndParent, RECT[] pRect, int bModal) {
            NativeMethods.SetParent(Control.Handle, hWndParent);
        }

        int IPropertyPage.Apply() {
            try {
                Apply();
                return VSConstants.S_OK;
            } catch (Exception e) {
                return Marshal.GetHRForException(e);
            }
        }

        void IPropertyPage.Deactivate() {
            Control.Dispose();
        }

        void IPropertyPage.GetPageInfo(PROPPAGEINFO[] pPageInfo) {
            if (pPageInfo == null) {
                throw new ArgumentNullException("arrInfo");
            }

            PROPPAGEINFO info = new PROPPAGEINFO();

            info.cb = (uint)Marshal.SizeOf(typeof(PROPPAGEINFO));
            info.dwHelpContext = 0;
            info.pszDocString = null;
            info.pszHelpFile = null;
            info.pszTitle = Name;
            info.SIZE.cx = Control.Width;
            info.SIZE.cy = Control.Height;
            pPageInfo[0] = info;
        }

        void IPropertyPage.Help(string pszHelpDir) {
        }

        int IPropertyPage.IsPageDirty() {
            return (IsDirty ? (int)VSConstants.S_OK : (int)VSConstants.S_FALSE);
        }

        void IPropertyPage.Move(RECT[] pRect) {
            if (pRect == null) {
                throw new ArgumentNullException("arrRect");
            }

            RECT r = pRect[0];

            Control.Location = new Point(r.left, r.top);
            Control.Size = new Size(r.right - r.left, r.bottom - r.top);
        }

        void IPropertyPage.SetObjects(uint count, object[] punk) {
            if (punk == null) {
                return;
            }

            if (count > 0) {
                if (punk[0] is ProjectConfig) {
                    ArrayList configs = new ArrayList();

                    for (int i = 0; i < count; i++) {
                        ProjectConfig config = (ProjectConfig)punk[i];

                        if (_project == null) {
                            _project = config.ProjectMgr;
                            break;
                        }

                        configs.Add(config);
                    }
                } else if (punk[0] is NodeProperties) {
                    if (_project == null) {
                        _project = (punk[0] as NodeProperties).Node.ProjectMgr;
                    }
                }
            } else {
                _project = null;
            }

            if (_project != null) {
                LoadSettings();
            }
        }

        void IPropertyPage.SetPageSite(IPropertyPageSite pPageSite) {
            _site = pPageSite;
        }

        void IPropertyPage.Show(uint nCmdShow) {
            Control.Visible = true; // TODO: pass SW_SHOW* flags through      
            Control.Show();
        }

        int IPropertyPage.TranslateAccelerator(MSG[] pMsg) {
            if (pMsg == null) {
                throw new ArgumentNullException("arrMsg");
            }

            MSG msg = pMsg[0];

            if ((msg.message < NativeMethods.WM_KEYFIRST || msg.message > NativeMethods.WM_KEYLAST) && (msg.message < NativeMethods.WM_MOUSEFIRST || msg.message > NativeMethods.WM_MOUSELAST)) {
                return VSConstants.S_FALSE;
            }

            return (NativeMethods.IsDialogMessageA(Control.Handle, ref msg)) ? VSConstants.S_OK : VSConstants.S_FALSE;
        }
    }
}
