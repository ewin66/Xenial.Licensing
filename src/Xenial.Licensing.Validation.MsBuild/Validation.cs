using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Build.Framework;

namespace Xenial.Licensing
{
    public class Validation : Microsoft.Build.Utilities.Task, ICancelableTask
    {
        private bool cancelled;
        public void Cancel() => cancelled = true;
        public override bool Execute()
        {
#if DEBUG
            // In Visual Studio or Visual Studio Code, you can add a breakpoint to this file.
            // Then, run MSBuild and use the "Attach to Process" feature to attach to the process
            // ID that this prints to the console.

            // Obviously, remove this when you're finished debugging as it will wait indefinitely
            // for the debugger to attach.
            System.Console.WriteLine("PID = " + System.Diagnostics.Process.GetCurrentProcess().Id);
            while (!System.Diagnostics.Debugger.IsAttached && !cancelled)
            {
            }
#endif



            return true;
        }
    }
}
