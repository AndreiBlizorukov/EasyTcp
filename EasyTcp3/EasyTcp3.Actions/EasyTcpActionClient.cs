using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyTcp3.Actions
{
    /// <summary>
    /// EasyTcpClient that supports 'actions'
    /// public static methods with the parameters (object, Message) and the EasyTcpAction attribute get automatically loaded as actions when class is constructed,
    /// more actions can be loaded with the AddActions method.
    /// </summary>
    public class EasyTcpActionClient : EasyTcpClient
    {
        /// <summary>
        /// Dictionary with all actions of this client [action code, action delegate]
        /// </summary>
        protected readonly Dictionary<int, ActionsCore.EasyTcpActionDelegate> Actions =
            new Dictionary<int, ActionsCore.EasyTcpActionDelegate>();

        /// <summary>
        /// Function that gets called before action is executed. If function returns false discard action. Ignored when null
        /// </summary>
        public Func<int, Message, bool> Interceptor;

        /// <summary>
        /// Action that gets triggered when an unknown action is received
        /// </summary>
        public event EventHandler<Message> OnUnknownAction;

        /// <summary>
        /// Function used to fire the OnUnknownAction event
        /// </summary>
        /// <param name="e">received message [int: action id][data]</param>
        protected void FireOnUnknownAction(Message e) => OnUnknownAction?.Invoke(this, e);

        /// <summary>
        /// Load new actions from an assembly
        /// </summary>
        /// <param name="assembly">assembly with EasyTcpActions</param>
        /// <param name="nameSpace">filter for namespace with EasyTcpActions.
        /// All actions in this namespace will be added, other will be ignored.
        /// Filter is ignored when null</param>
        public void AddActions(Assembly assembly, string nameSpace = null)
        {
            foreach (var action in ActionsCore.GetActions(assembly ?? Assembly.GetCallingAssembly(), nameSpace))
                Actions.Add(action.Key, action.Value);
        }

        /// <summary>
        /// Create new EasyTcpActionClient and load actions from an assembly
        /// </summary>
        /// <param name="assembly">assembly with EasyTcpActions, calling assembly when null</param>
        /// <param name="nameSpace">filter for namespace with EasyTcpActions.
        /// All actions in this namespace will be added, other will be ignored.
        /// Filter is ignored when null</param>
        public EasyTcpActionClient(Assembly assembly = null, string nameSpace = null)
        {
            AddActions(assembly ?? Assembly.GetCallingAssembly(), nameSpace);
            OnDataReceive += (sender, message) =>
                Actions.ExecuteAction(Interceptor, FireOnUnknownAction, sender, message);
        }
    }
}