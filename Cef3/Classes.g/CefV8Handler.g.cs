//
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
//
namespace Cef3
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Cef3.Interop;
    
    // Role: HANDLER
    public abstract unsafe partial class CefV8Handler
    {
        private static Dictionary<IntPtr, CefV8Handler> _roots = new Dictionary<IntPtr, CefV8Handler>();
        
        private int _refct;
        private cef_v8handler_t* _self;
        
        protected object SyncRoot { get { return this; } }
        
        internal static CefV8Handler FromNativeOrNull(cef_v8handler_t* ptr)
        {
            CefV8Handler value = null;
            bool found;
            lock (_roots)
            {
                found = _roots.TryGetValue((IntPtr)ptr, out value);
            }
            return found ? value : null;
        }
        
        internal static CefV8Handler FromNative(cef_v8handler_t* ptr)
        {
            var value = FromNativeOrNull(ptr);
            if (value == null) throw ExceptionBuilder.ObjectNotFound();
            return value;
        }
        
        private cef_v8handler_t.add_ref_delegate _ds0;
        private cef_v8handler_t.release_delegate _ds1;
        private cef_v8handler_t.get_refct_delegate _ds2;
        private cef_v8handler_t.execute_delegate _ds3;
        
        protected CefV8Handler()
        {
            _self = cef_v8handler_t.Alloc();
        
            _ds0 = new cef_v8handler_t.add_ref_delegate(add_ref);
            _self->_base._add_ref = Marshal.GetFunctionPointerForDelegate(_ds0);
            _ds1 = new cef_v8handler_t.release_delegate(release);
            _self->_base._release = Marshal.GetFunctionPointerForDelegate(_ds1);
            _ds2 = new cef_v8handler_t.get_refct_delegate(get_refct);
            _self->_base._get_refct = Marshal.GetFunctionPointerForDelegate(_ds2);
            _ds3 = new cef_v8handler_t.execute_delegate(execute);
            _self->_execute = Marshal.GetFunctionPointerForDelegate(_ds3);
        }
        
        ~CefV8Handler()
        {
            Dispose(false);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_self != null)
            {
                cef_v8handler_t.Free(_self);
                _self = null;
            }
        }
        
        private int add_ref(cef_v8handler_t* self)
        {
            lock (SyncRoot)
            {
                var result = ++_refct;
                if (result == 1)
                {
                    lock (_roots) { _roots.Add((IntPtr)_self, this); }
                }
                return result;
            }
        }
        
        private int release(cef_v8handler_t* self)
        {
            lock (SyncRoot)
            {
                var result = --_refct;
                if (result == 0)
                {
                    lock (_roots) { _roots.Remove((IntPtr)_self); }
                }
                return result;
            }
        }
        
        private int get_refct(cef_v8handler_t* self)
        {
            return _refct;
        }
        
        internal cef_v8handler_t* ToNative()
        {
            add_ref(_self);
            return _self;
        }
        
        [Conditional("DEBUG")]
        private void CheckSelf(cef_v8handler_t* self)
        {
            if (_self != self) throw ExceptionBuilder.InvalidSelfReference();
        }
        
    }
}
