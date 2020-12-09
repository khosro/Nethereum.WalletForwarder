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
using Nethereum.WalletForwarder.Contracts.Forwarder.ContractDefinition;

namespace Nethereum.WalletForwarder.Contracts.Forwarder
{
    public partial class LazyForwarderService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync<T>
            (Nethereum.Web3.Web3 web3, T forwarderDeployment, CancellationTokenSource cancellationTokenSource = null)
            where T : ContractDeploymentMessage, new()
        {
            return web3.Eth.GetContractDeploymentHandler<T>().SendRequestAndWaitForReceiptAsync(forwarderDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync<T>(Nethereum.Web3.Web3 web3, T forwarderDeployment)
                        where T : ContractDeploymentMessage, new()
        {
            return web3.Eth.GetContractDeploymentHandler<T>().SendRequestAsync(forwarderDeployment);
        }

        public static async Task<LazyForwarderService> DeployContractAndGetServiceAsync<T>(Nethereum.Web3.Web3 web3, T forwarderDeployment, CancellationTokenSource cancellationTokenSource = null)
                                    where T : ContractDeploymentMessage, new()
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, forwarderDeployment, cancellationTokenSource);
            return new LazyForwarderService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3 { get; }

        public ContractHandler ContractHandler { get; }

        public LazyForwarderService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        /* 

        public Task<TransactionReceipt> ChangeDestinationRequestAndWaitForReceiptAsync(LazyChangeDestinationFunction changeDestinationFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(changeDestinationFunction, cancellationToken);
        } 
 
        public Task<string> DestinationQueryAsync(LazyDestinationFunction destinationFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<LazyDestinationFunction, string>(destinationFunction, blockParameter);
        }  */
 
 
        public Task<TransactionReceipt> FlushRequestAndWaitForReceiptAsync(LazyFlushFunction flushFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(flushFunction, cancellationToken);
        }
 
        public Task<TransactionReceipt> FlushTokensRequestAndWaitForReceiptAsync(LazyFlushTokensFunction flushTokensFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(flushTokensFunction, cancellationToken);
        }
 
        public Task<TransactionReceipt> InitRequestAndWaitForReceiptAsync(LazyInitFunction initFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(initFunction, cancellationToken);
        }
 

        //public Task<TransactionReceipt> WithdrawRequestAndWaitForReceiptAsync(LazyWithdrawFunction withdrawFunction, CancellationTokenSource cancellationToken = null)
        //{
        //    return ContractHandler.SendRequestAndWaitForReceiptAsync(withdrawFunction, cancellationToken);
        //}
 
    }
}
