using System;
using System.ServiceModel;

namespace Microsoft.Windows.Shell
{
    [ServiceContract]
    interface ISingleInstanceService
    {
        [OperationContract(IsOneWay=true)]
        void BringToFront(Guid appGuid);

        [OperationContract(IsOneWay=true)]
        void ProcessArguments(Guid appGuid, string[] args);
    }

    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerSession, 
        ConcurrencyMode=ConcurrencyMode.Single, 
        UseSynchronizationContext=true)]
    class SingleInstanceService : ISingleInstanceService
    {
        private Action<Guid> bringToFrontCallback;
        private Action<Guid, string[]> processArgsCallback;

        public SingleInstanceService()
        {
        }

        public SingleInstanceService(Action<Guid> bringToFrontCallback, 
                                     Action<Guid, string[]> processArgsCallback)
        {
            this.bringToFrontCallback = bringToFrontCallback;
            this.processArgsCallback = processArgsCallback;
        }
        
        public void BringToFront(Guid appGuid)
        {
            bringToFrontCallback(appGuid);
        }

        public void ProcessArguments(Guid appGuid, string[] args)
        {
            processArgsCallback(appGuid, args);
        }
    }
}
