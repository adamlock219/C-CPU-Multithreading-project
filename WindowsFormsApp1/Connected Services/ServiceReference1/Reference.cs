﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WindowsFormsApp1.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IService")]
    public interface IService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/AlgorithmOne", ReplyAction="http://tempuri.org/IService/AlgorithmOneResponse")]
        string AlgorithmOne(AlgorithmLibrary.ConfigData cd);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/AlgorithmOne", ReplyAction="http://tempuri.org/IService/AlgorithmOneResponse")]
        System.Threading.Tasks.Task<string> AlgorithmOneAsync(AlgorithmLibrary.ConfigData cd);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChannel : WindowsFormsApp1.ServiceReference1.IService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceClient : System.ServiceModel.ClientBase<WindowsFormsApp1.ServiceReference1.IService>, WindowsFormsApp1.ServiceReference1.IService {
        
        public ServiceClient() {
        }
        
        public ServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string AlgorithmOne(AlgorithmLibrary.ConfigData cd) {
            return base.Channel.AlgorithmOne(cd);
        }
        
        public System.Threading.Tasks.Task<string> AlgorithmOneAsync(AlgorithmLibrary.ConfigData cd) {
            return base.Channel.AlgorithmOneAsync(cd);
        }
    }
}