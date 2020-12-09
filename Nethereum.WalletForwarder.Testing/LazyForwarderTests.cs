//using System.Numerics;
//using Nethereum.Signer;
//using Nethereum.Util;
//using Nethereum.Web3.Accounts;
//using Nethereum.XUnitEthereumClients;
//using Xunit;
//using Nethereum.WalletForwarder.Contracts.Forwarder;
//using Nethereum.WalletForwarder.Contracts.Forwarder.ContractDefinition;
//using Nethereum.WalletForwarder.Contracts.ForwarderFactory;
//using Nethereum.WalletForwarder.Contracts.ForwarderFactory.ContractDefinition;
//using Nethereum.Hex.HexConvertors.Extensions;
//using Nethereum.RLP;
//using Nethereum.Contracts;
//using Nethereum.ABI.Encoders;
//using Nethereum.ABI.Decoders;
//using System.Globalization;
//using System.Collections.Generic;
//using Nethereum.WalletForwarder.Contracts.ERC20Token;
//using Nethereum.WalletForwarder.Contracts.ERC20Token.ContractDefinition;
//using System.Threading.Tasks;
//using System;
//using System.Collections.Concurrent;
//using System.Linq;
//using Nethereum.RPC.Eth.DTOs;
//using System.Text;
//using NBitcoin;
//using FluentAssertions;
//using Nethereum.JsonRpc.Client;
//using Microsoft.Extensions.Logging;
//using Xunit.Abstractions;

//namespace Nethereum.Forwarder.IntegrationTests
//{
//    [Collection(EthereumClientIntegrationFixture.ETHEREUM_CLIENT_COLLECTION_DEFAULT)]
//    public class LazyForwarderTests : ForwarderTestBase
//    {
//        private ITestOutputHelper OutputHelper { get; }

//        public LazyForwarderTests(EthereumClientIntegrationFixture ethereumClientIntegrationFixture,
//            ITestOutputHelper outputHelper) : base(ethereumClientIntegrationFixture)
//        {
//            OutputHelper = outputHelper;
//        }

//        #region ShouldDeployForwarder_CloneItUsingFactory_TransferEther

//        [Fact]
//        public async void ShouldDeployForwarder_CloneItUsingFactory_TransferEther_Salt_Is_Unit()
//        {
//            await ShouldDeployForwarder_CloneItUsingFactory_TransferEther(SaltType.Unint);
//        }

//        [Fact]
//        public async void ShouldDeployForwarder_CloneItUsingFactory_TransferEther_Salt_Is_String_Guid()
//        {
//            await ShouldDeployForwarder_CloneItUsingFactory_TransferEther(SaltType.String_Guid);
//        }

//        [Fact]
//        public async void ShouldDeployForwarder_CloneItUsingFactory_TransferEther_Salt_Is_String_NBitcoin_RandomUtils()
//        {
//            await ShouldDeployForwarder_CloneItUsingFactory_TransferEther(SaltType.String_NBitcoin_RandomUtils);
//        }


//        async Task ShouldDeployForwarder_CloneItUsingFactory_TransferEther(SaltType saltType)
//        {
//            var destinationAddress = "0x6C547791C3573c2093d81b919350DB1094707011";
//            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

//            //Getting the current Ether balance of the destination, we are going to transfer 0.001 ether
//            var balanceDestination = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            var balanceDestinationEther = Web3.Web3.Convert.FromWei(balanceDestination);

//            var defaultForwarderDeploymentReceipt = await ForwarderService.DeployContractAndWaitForReceiptAsync(web3, new LazyForwarderDeployment());
//            var defaultForwaderContractAddress = defaultForwarderDeploymentReceipt.ContractAddress;
//            var defaultForwarderService = new ForwarderService(web3, defaultForwaderContractAddress);
//            await defaultForwarderService.ChangeDestinationRequestAndWaitForReceiptAsync(destinationAddress);
//            var destinationInContract = await defaultForwarderService.DestinationQueryAsync();
//            Assert.True(destinationInContract.IsTheSameAddress(destinationAddress));

//            var factoryDeploymentReceipt = await ForwarderFactoryService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderFactoryDeployment());
//            var factoryAddress = factoryDeploymentReceipt.ContractAddress;
//            var factoryService = new ForwarderFactoryService(web3, factoryDeploymentReceipt.ContractAddress);

//            TransactionReceipt txnReceipt;
//            string saltHex;
//            if (saltType == SaltType.Unint)
//            {
//                //New invovice 
//                var salt = BigInteger.Parse("12");
//                saltHex = new IntTypeEncoder().Encode(salt).ToHex();

//                txnReceipt = await factoryService.CloneForwarderRequestAndWaitForReceiptAsync(new CloneForwarderFunction()
//                {
//                    Forwarder = defaultForwaderContractAddress,
//                    Salt = salt,
//                    IsLazy = true
//                });
//            }
//            else
//            {
//                //New invovice 
//                var salts = GetSalts(saltType);
//                var salt = salts.Salt;
//                saltHex = salts.SaltHax;

//                txnReceipt = await factoryService.CloneForwarder1RequestAndWaitForReceiptAsync(new CloneForwarder1Function()
//                {
//                    Forwarder = defaultForwaderContractAddress,
//                    Salt = salt,
//                    IsLazy = true
//                });
//            }

//            var contractCalculatedAddress = CalculateCreate2AddressMinimalProxy(factoryAddress, saltHex, defaultForwaderContractAddress);

//            var clonedAdress = txnReceipt.DecodeAllEvents<ForwarderClonedEventDTO>()[0].Event.ClonedAdress;
//            Assert.True(clonedAdress.IsTheSameAddress(contractCalculatedAddress));


//            var clonedForwarderService = new ForwarderService(web3, contractCalculatedAddress);
//            var destinationInContractCloned = await clonedForwarderService.DestinationQueryAsync();
//            Assert.True(destinationInContractCloned.IsTheSameAddress(destinationAddress));

//            //gas is added due to forwarding
//            var transferEtherReceipt = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractCalculatedAddress, 10, null, 4500000);
//            // var forwardedDeposit = transferEtherReceipt.DecodeAllEvents<ForwarderDepositedEventDTO>()[0].Event;


//            #region Difference with the same method in ForwarderTests

//            var balance = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            Assert.Equal(balanceDestinationEther, Web3.Web3.Convert.FromWei(balance));

//            await factoryService.FlushEtherRequestAndWaitForReceiptAsync(new List<string> { contractCalculatedAddress });

//            #endregion

//            balance = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            Assert.Equal(balanceDestinationEther + 10, Web3.Web3.Convert.FromWei(balance));
//            var balance2 = await web3.Eth.GetBalance.SendRequestAsync(contractCalculatedAddress);
//            Assert.Equal(0, balance2.Value);

//        }

//        #endregion


//        #region

//        [Fact]
//        public async void ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEtherManyClonesUsingFactory_Salt_Is_Unit()
//        {
//            await ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEtherManyClonesUsingFactory(SaltType.Unint);
//        }

//        [Fact]
//        public async void ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEtherManyClonesUsingFactory_Salt_Is_String_Guid()
//        {
//            await ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEtherManyClonesUsingFactory(SaltType.String_Guid);
//        }

//        [Fact]
//        public async void ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEtherManyClonesUsingFactory_Salt_Is_String_NBitcoin_RandomUtils()
//        {
//            await ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEtherManyClonesUsingFactory(SaltType.String_NBitcoin_RandomUtils);
//        }

//        async Task ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEtherManyClonesUsingFactory(SaltType saltType)
//        {
//            BigInteger sumOfCreatingContractGas = 0;

//            var destinationAddress = "0x6C547791C3573c2093d81b919350DB1094707011";
//            //Using ropsten infura 
//            //var web3 = _ethereumClientIntegrationFixture.GetInfuraWeb3(InfuraNetwork.Ropsten);
//            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

//            //Getting the current Ether balance of the destination, we are going to transfer 0.001 ether
//            var balanceDestination = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            var balanceDestinationEther = Web3.Web3.Convert.FromWei(balanceDestination);

//            //Deploying first the default forwarder (template for all clones)
//            var defaultForwarderDeploymentReceipt = await ForwarderService.DeployContractAndWaitForReceiptAsync(web3, new LazyForwarderDeployment());
//            var defaultForwaderContractAddress = defaultForwarderDeploymentReceipt.ContractAddress;
//            var defaultForwarderService = new ForwarderService(web3, defaultForwaderContractAddress);
//            //initialiasing with the destination address
//            await defaultForwarderService.ChangeDestinationRequestAndWaitForReceiptAsync(destinationAddress);
//            var destinationInContract = await defaultForwarderService.DestinationQueryAsync();
//            //validate the destination address has been set correctly
//            Assert.True(destinationInContract.IsTheSameAddress(destinationAddress));

//            sumOfCreatingContractGas += defaultForwarderDeploymentReceipt.GasUsed;

//            //Deploying the factory
//            var factoryDeploymentReceipt = await ForwarderFactoryService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderFactoryDeployment());
//            var factoryAddress = factoryDeploymentReceipt.ContractAddress;
//            var factoryService = new ForwarderFactoryService(web3, factoryDeploymentReceipt.ContractAddress);

//            sumOfCreatingContractGas += factoryDeploymentReceipt.GasUsed;

//            var addressesAndGasUsed = await SendEtherAndCreateClones(10, web3, factoryService, 0.001M, factoryAddress, defaultForwaderContractAddress, saltType);


//            #region Difference with the same method in ForwarderTests

//            var balance = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            Assert.Equal(balanceDestinationEther, Web3.Web3.Convert.FromWei(balance));

//            #endregion

//            //Flushing from the factory
//            var flushAllReceipt = await factoryService.FlushEtherRequestAndWaitForReceiptAsync(addressesAndGasUsed.ClonedAddressess);
//            //check here the cost ^^^
//            var totalEtherTransfered = 0.001M * addressesAndGasUsed.ClonedAddressess.Count;

//            var newbalanceDestination = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            Assert.Equal(totalEtherTransfered + balanceDestinationEther, Web3.Web3.Convert.FromWei(newbalanceDestination));

//            StringBuilder stringBuilder = new StringBuilder();

//            stringBuilder
//                .AppendLine($"SaltType                            : {saltType}")
//                .AppendLine($"Sum Of Creating Contract Gas Used   : {sumOfCreatingContractGas.ToString("#,##0.00")}")
//                .AppendLine($"Sum Of Cloned Gas Used              : {addressesAndGasUsed.SumOfClonedGas.ToString("#,##0.00")}")
//                .AppendLine($"SumOf Ordinary Transfer Gas Used    : {addressesAndGasUsed.SumOfOrdinaryTransferGasUsed.ToString("#,##0.00")}")
//                .AppendLine($"Flush Ether Gas Used                : {flushAllReceipt.GasUsed.Value.ToString("#,##0.00")}");


//            OutputHelper.WriteLine(stringBuilder.ToString());
//        }

//        private async Task<(List<string> ClonedAddressess, BigInteger SumOfClonedGas, BigInteger SumOfOrdinaryTransferGasUsed)> SendEtherAndCreateClones(int numberOfClones, Web3.Web3 web3, ForwarderFactoryService factoryService, decimal amount, string factoryAddress, string defaultForwaderContractAddress, SaltType saltType)
//        {
//            BigInteger sumOfClonedGas = 0;
//            BigInteger sumOfOrdinaryTransferGasUsed = 0;

//            var numProcs = Environment.ProcessorCount;
//            var concurrencyLevel = numProcs * 2;
//            var concurrentDictionary = new ConcurrentDictionary<int, string>(concurrencyLevel, numberOfClones * 2);
//            var taskItems = new List<int>();
//            for (var i = 0; i < numberOfClones; i++)
//                taskItems.Add(i);

//            Parallel.ForEach(taskItems, (item, state) =>
//            {
//                var id = item.ToString();
//                var addressAndGas = SendEtherAndCreateClone(web3, factoryService, amount, id, factoryAddress, defaultForwaderContractAddress, saltType).Result;
//                concurrentDictionary.TryAdd(item, addressAndGas.CloneAddresss);
//                sumOfClonedGas += addressAndGas.CloneAddresssGasUsed;
//                sumOfOrdinaryTransferGasUsed += addressAndGas.OrdinaryTransferGasUsed;
//            });

//            return (concurrentDictionary.Values.ToList(), sumOfClonedGas, sumOfOrdinaryTransferGasUsed);
//        }


//        private async Task<(string CloneAddresss, BigInteger CloneAddresssGasUsed, BigInteger OrdinaryTransferGasUsed)> SendEtherAndCreateClone(Web3.Web3 web3, ForwarderFactoryService factoryService, decimal amount, string saltNumber, string factoryAddress, string defaultForwaderContractAddress, SaltType saltType)
//        {
//            TransactionReceipt txnReceipt;
//            TransactionReceipt transferEtherReceipt;
//            string contractCalculatedAddress;

//            if (saltType == SaltType.Unint)
//            {
//                //Lets create new contract to be paid
//                var salt = BigInteger.Parse(saltNumber); //salt id
//                var saltHex = new IntTypeEncoder().Encode(salt).ToHex();

//                //Calculate the new contract address
//                contractCalculatedAddress = CalculateCreate2AddressMinimalProxy(factoryAddress, saltHex, defaultForwaderContractAddress);

//                //Let's tranfer some ether, with some extra gas to allow forwarding if the smart contract is deployed (UX problem)
//                txnReceipt = await factoryService.CloneForwarderRequestAndWaitForReceiptAsync(
//                    new CloneForwarderFunction()
//                    {
//                        Forwarder = defaultForwaderContractAddress,
//                        Salt = salt,
//                        IsLazy = true
//                    });

//                transferEtherReceipt = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractCalculatedAddress, amount, null, 4500000);
//            }
//            else
//            {
//                var salts = GetSalts(saltType);
//                var salt = salts.Salt;
//                var saltHex = salts.SaltHax;
//                //Calculate the new contract address
//                contractCalculatedAddress = CalculateCreate2AddressMinimalProxy(factoryAddress, saltHex, defaultForwaderContractAddress);

//                //Let's tranfer some ether, with some extra gas to allow forwarding if the smart contract is deployed (UX problem)
//                txnReceipt = await factoryService.CloneForwarder1RequestAndWaitForReceiptAsync(new CloneForwarder1Function()
//                {
//                    Forwarder = defaultForwaderContractAddress,
//                    Salt = salt,
//                    IsLazy = true
//                });

//                transferEtherReceipt = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractCalculatedAddress, amount, null, 4500000);

//            }


//            var clonedAdress = txnReceipt.DecodeAllEvents<ForwarderClonedEventDTO>()[0].Event.ClonedAdress;
//            Assert.True(clonedAdress.IsTheSameAddress(contractCalculatedAddress));

//            return (contractCalculatedAddress, txnReceipt.GasUsed, transferEtherReceipt.GasUsed);
//        }

//        #endregion

//    }
//}