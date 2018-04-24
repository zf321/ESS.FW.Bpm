//using System;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Identity;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;

//namespace ESS.FW.Bpm.Engine.Impl.Cmd
//{
//    /// <summary>
//    ///      
//    /// </summary>
//    [Serializable]
//    public class CreateUserQueryCmd : ICommand<IQueryable<IUser>>
//    {
//        private const long SerialVersionUid = 1L;

//        public virtual IQueryable<IUser> Execute(CommandContext commandContext)
//        {
//            return commandContext.ReadOnlyIdentityProvider.CreateUserQuery();
//        }
//    }
//}