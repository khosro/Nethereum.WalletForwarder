using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using Nethereum.WalletForwarder.Contracts.ForwarderFactory.ContractDefinition;

namespace Nethereum.WalletForwarder.Contracts.ForwarderFactory
{
    public partial class LazyForwarderFactoryService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, LazyForwarderFactoryDeployment forwarderFactoryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<LazyForwarderFactoryDeployment>().SendRequestAndWaitForReceiptAsync(forwarderFactoryDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, LazyForwarderFactoryDeployment forwarderFactoryDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<LazyForwarderFactoryDeployment>().SendRequestAsync(forwarderFactoryDeployment);
        }

        public static async Task<LazyForwarderFactoryService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, LazyForwarderFactoryDeployment forwarderFactoryDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, forwarderFactoryDeployment, cancellationTokenSource);
            return new LazyForwarderFactoryService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3 { get; }

        public ContractHandler ContractHandler { get; }

        public LazyForwarderFactoryService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<TransactionReceipt> CloneForwarderRequestAndWaitForReceiptAsync(LazyCloneForwarderFunction cloneForwarderFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(cloneForwarderFunction, cancellationToken);
        }
 
        public Task<TransactionReceipt> FlushEtherRequestAndWaitForReceiptAsync(LazyFlushEtherFunction flushEtherFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(flushEtherFunction, cancellationToken);
        }
 

        public Task<TransactionReceipt> FlushTokensRequestAndWaitForReceiptAsync(LazyFlushTokensFunction flushTokensFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(flushTokensFunction, cancellationToken);
        }

  
    }
}
