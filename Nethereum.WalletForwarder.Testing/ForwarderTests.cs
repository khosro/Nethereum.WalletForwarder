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

//namespace Nethereum.Forwarder.IntegrationTests
//{
//    [Collection(EthereumClientIntegrationFixture.ETHEREUM_CLIENT_COLLECTION_DEFAULT)]
//    public class ForwarderTests : ForwarderTestBase
//    {
//        public ForwarderTests(EthereumClientIntegrationFixture ethereumClientIntegrationFixture) : base(ethereumClientIntegrationFixture)
//        {
//        }

//        [Fact]
//        public async void ShouldDeployForwarder_CloneMustBeDoneOnlyWithOwner()
//        {
//            var destinationAddress = "0x6C547791C3573c2093d81b919350DB1094707011";
//            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

//            await TransferToAlternateAccount(web3);

//            //Getting the current Ether balance of the destination, we are going to transfer 0.001 ether
//            var balanceDestination = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            var balanceDestinationEther = Web3.Web3.Convert.FromWei(balanceDestination);

//            var defaultForwarderDeploymentReceipt = await ForwarderService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderDeployment());
//            var defaultForwaderContractAddress = defaultForwarderDeploymentReceipt.ContractAddress;
//            var defaultForwarderService = new ForwarderService(web3, defaultForwaderContractAddress);
//            await defaultForwarderService.ChangeDestinationRequestAndWaitForReceiptAsync(destinationAddress);
//            var destinationInContract = await defaultForwarderService.DestinationQueryAsync();
//            Assert.True(destinationInContract.IsTheSameAddress(destinationAddress));

//            var factoryDeploymentReceipt = await ForwarderFactoryService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderFactoryDeployment());
//            var factoryAddress = factoryDeploymentReceipt.ContractAddress;
//            var factoryService = new ForwarderFactoryService(_ethereumClientIntegrationFixture.GetAlternateWeb3(), factoryDeploymentReceipt.ContractAddress);

//            TransactionReceipt txnReceipt;
//            string saltHex;

//            //New invovice 
//            var salt = BigInteger.Parse("12");
//            saltHex = new IntTypeEncoder().Encode(salt).ToHex();

//            Func<Task> act = async () => { await factoryService.CloneForwarderRequestAndWaitForReceiptAsync(defaultForwaderContractAddress, salt); };

//            await act.Should().ThrowAsync<RpcResponseException>("execution reverted: Only owner");
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

//            var defaultForwarderDeploymentReceipt = await ForwarderService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderDeployment());
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

//                txnReceipt = await factoryService.CloneForwarderRequestAndWaitForReceiptAsync(defaultForwaderContractAddress, salt);
//            }
//            else
//            {
//                //New invovice 
//                var salts = GetSalts(saltType);
//                var salt = salts.Salt;
//                saltHex = salts.SaltHax;

//                txnReceipt = await factoryService.CloneForwarder1RequestAndWaitForReceiptAsync(defaultForwaderContractAddress, salt);
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

//            var balance = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            Assert.Equal(balanceDestinationEther + 10, Web3.Web3.Convert.FromWei(balance));
//            var balance2 = await web3.Eth.GetBalance.SendRequestAsync(contractCalculatedAddress);
//            Assert.Equal(0, balance2.Value);

//        }

//        #endregion

//        #region  ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther

//        [Fact]
//        public async void ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther_Salt_Is_Unit()
//        {
//            await ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther(SaltType.Unint);
//        }


//        [Fact]
//        public async void ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther_Salt_Is_String_Guid()
//        {
//            await ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther(SaltType.String_Guid);
//        }


//        [Fact]
//        public async void ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther_Salt_Is_String_NBitcoin_RandomUtils()
//        {
//            await ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther(SaltType.String_NBitcoin_RandomUtils);
//        }

//        [Fact]
//        public async void ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEtherByNotOwnerAndDestination_Salt_Is_String_NBitcoin_RandomUtils()
//        {
//            await ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther(SaltType.String_NBitcoin_RandomUtils, false);
//        }

//        async Task ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther(SaltType saltType, bool isOwner = true)
//        {
//            var destinationAddress = EthereumClientIntegrationFixture.DestinationPublicKey;
//            //Using ropsten infura 
//            //var web3 = _ethereumClientIntegrationFixture.GetInfuraWeb3(InfuraNetwork.Ropsten);
//            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

//            //Getting the current Ether balance of the destination, we are going to transfer 0.001 ether
//            var balanceDestination = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            var balanceDestinationEther = Web3.Web3.Convert.FromWei(balanceDestination);

//            //Deploying first the default forwarder (template for all clones)
//            var defaultForwarderDeploymentReceipt = await ForwarderService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderDeployment());
//            var defaultForwaderContractAddress = defaultForwarderDeploymentReceipt.ContractAddress;
//            var defaultForwarderService = new ForwarderService(web3, defaultForwaderContractAddress);
//            //initialiasing with the destination address
//            await defaultForwarderService.ChangeDestinationRequestAndWaitForReceiptAsync(destinationAddress);
//            var destinationInContract = await defaultForwarderService.DestinationQueryAsync();
//            //validate the destination address has been set correctly
//            Assert.True(destinationInContract.IsTheSameAddress(destinationAddress));

//            //Deploying the factory
//            var factoryDeploymentReceipt = await ForwarderFactoryService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderFactoryDeployment());
//            var factoryAddress = factoryDeploymentReceipt.ContractAddress;
//            var factoryService = new ForwarderFactoryService(web3, factoryDeploymentReceipt.ContractAddress);


//            BigInteger saltBbigInteger = 0;
//            byte[] saltByte = null;
//            string saltHex;
//            if (saltType == SaltType.Unint)
//            {
//                //Lets create new invovice to be paid
//                saltBbigInteger = BigInteger.Parse("12");
//                saltHex = new IntTypeEncoder().Encode(saltBbigInteger).ToHex();
//            }
//            else
//            {
//                //Lets create new invovice to be paid
//                var salts = GetSalts(saltType);
//                saltByte = salts.Salt;
//                saltHex = salts.SaltHax;
//            }

//            //Calculate the new contract address
//            var contractCalculatedAddress = CalculateCreate2AddressMinimalProxy(factoryAddress, saltHex, defaultForwaderContractAddress);

//            //Let's tranfer some ether, with some extra gas to allow forwarding if the smart contract is deployed (UX problem)
//            var transferEtherReceipt = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractCalculatedAddress, (decimal)0.001, null, 4500000);


//            //Check the balance of the adress we sent.. we have not deployed the smart contract so it should be still the same
//            var balanceContract = await web3.Eth.GetBalance.SendRequestAsync(contractCalculatedAddress);
//            //Assert.Equal((decimal)0.001, Web3.Web3.Convert.FromWei(balanceContract.Value));

//            TransactionReceipt txnReceipt;

//            if (saltType == SaltType.Unint)
//            {
//                //Create the clone with the salt to match the address
//                txnReceipt = await factoryService.CloneForwarderRequestAndWaitForReceiptAsync(defaultForwaderContractAddress, saltBbigInteger);
//            }
//            else
//            {
//                //Create the clone with the salt to match the address
//                txnReceipt = await factoryService.CloneForwarder1RequestAndWaitForReceiptAsync(defaultForwaderContractAddress, saltByte);
//            }


//            var clonedAdress = txnReceipt.DecodeAllEvents<ForwarderClonedEventDTO>()[0].Event.ClonedAdress;
//            Assert.True(clonedAdress.IsTheSameAddress(contractCalculatedAddress));


//            //we should still have the same balance
//            balanceContract = await web3.Eth.GetBalance.SendRequestAsync(contractCalculatedAddress);
//            Assert.Equal((decimal)0.001, Web3.Web3.Convert.FromWei(balanceContract.Value));

//            //create a service to for cloned forwarder
//            var clonedForwarderService = new ForwarderService(web3, contractCalculatedAddress);
//            var destinationInContractCloned = await clonedForwarderService.DestinationQueryAsync();
//            //validate the destination address is the same
//            Assert.True(destinationInContractCloned.IsTheSameAddress(destinationAddress));

//            var ownerContractCloned = await clonedForwarderService.OwnerQueryAsync();
//            Console.WriteLine($"ownerContractCloned {ownerContractCloned}");

//            //Khosro

//            //Using flush directly in the cloned contract
//            //call flush to get all the ether transferred to destination address 
//            if (isOwner)
//            {
//                clonedForwarderService = new ForwarderService(_ethereumClientIntegrationFixture.GetDestinationWeb3(), contractCalculatedAddress);

//                await TransferToDestination(web3);
//                balanceDestinationEther = Web3.Web3.Convert.FromWei(balanceDestination);

//                //TODO. Subtract gas amount * gas price used in FlushRequestAndWaitForReceiptAsync function  from balanceDestinationEther
//                var receipt = await clonedForwarderService.FlushRequestAndWaitForReceiptAsync();
//                //await web3.Eth.TransactionManager.Transact
//            }
//            else
//            {
//                Func<Task> func = async () => { await clonedForwarderService.FlushRequestAndWaitForReceiptAsync(); };
//                await func.Should().ThrowAsync<RpcResponseException>("execution reverted: Only destination and owner");
//                return;
//            }

//            balanceContract = await web3.Eth.GetBalance.SendRequestAsync(contractCalculatedAddress);
//            //validate balances...
//            var newbalanceDestination = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            Assert.Equal((decimal)0.001 + balanceDestinationEther, Web3.Web3.Convert.FromWei(newbalanceDestination));
//        }

//        #endregion

//        #region ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther2ClonesUsingFactory

//        [Fact]
//        public async void ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther2ClonesUsingFactory_FlushWithAnotherAccount_Salt_Is_Unit()
//        {
//            await ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther2ClonesUsingFactory(SaltType.Unint, true);
//        }

//        [Fact]
//        public async void ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther2ClonesUsingFactory_Salt_Is_Unit()
//        {
//            await ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther2ClonesUsingFactory(SaltType.Unint);
//        }

//        [Fact]
//        public async void ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther2ClonesUsingFactory_Salt_Is_String_Guid()
//        {
//            await ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther2ClonesUsingFactory(SaltType.String_Guid);
//        }


//        [Fact]
//        public async void ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther2ClonesUsingFactory_Salt_Is_String_NBitcoin_RandomUtils()
//        {
//            await ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther2ClonesUsingFactory(SaltType.String_NBitcoin_RandomUtils);
//        }


//        async Task ShouldDeployForwarder_TransferEther_CloneItUsingFactory_FlushEther2ClonesUsingFactory(SaltType saltType, bool isUsedAnotherAccount = false)
//        {
//            var destinationAddress = "0x6C547791C3573c2093d81b919350DB1094707011";
//            //Using ropsten infura 
//            //var web3 = _ethereumClientIntegrationFixture.GetInfuraWeb3(InfuraNetwork.Ropsten);
//            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

//            //Getting the current Ether balance of the destination, we are going to transfer 0.001 ether
//            var balanceDestination = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            var balanceDestinationEther = Web3.Web3.Convert.FromWei(balanceDestination);

//            //Deploying first the default forwarder (template for all clones)
//            var defaultForwarderDeploymentReceipt = await ForwarderService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderDeployment());
//            var defaultForwaderContractAddress = defaultForwarderDeploymentReceipt.ContractAddress;
//            var defaultForwarderService = new ForwarderService(web3, defaultForwaderContractAddress);
//            //initialiasing with the destination address
//            await defaultForwarderService.ChangeDestinationRequestAndWaitForReceiptAsync(destinationAddress);
//            var destinationInContract = await defaultForwarderService.DestinationQueryAsync();
//            //validate the destination address has been set correctly
//            Assert.True(destinationInContract.IsTheSameAddress(destinationAddress));

//            //Deploying the factory
//            var factoryDeploymentReceipt = await ForwarderFactoryService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderFactoryDeployment());
//            var factoryAddress = factoryDeploymentReceipt.ContractAddress;
//            var factoryService = new ForwarderFactoryService(web3, factoryDeploymentReceipt.ContractAddress);

//            string contractCalculatedAddress;


//            string contractCalculatedAddress2;

//            if (saltType == SaltType.Unint)
//            {
//                //Lets create new invovice to be paid
//                var saltBbigInteger = BigInteger.Parse("12");
//                var saltHex = new IntTypeEncoder().Encode(saltBbigInteger).ToHex();
//                //Calculate the new contract address
//                contractCalculatedAddress = CalculateCreate2AddressMinimalProxy(factoryAddress, saltHex, defaultForwaderContractAddress);
//                //Let's tranfer some ether, with some extra gas to allow forwarding if the smart contract is deployed (UX problem)
//                var transferEtherReceipt = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractCalculatedAddress, (decimal)0.001, null, 4500000);


//                //Lets create new invovice to be paid
//                var saltBbigInteger2 = BigInteger.Parse("13");
//                var saltHex2 = new IntTypeEncoder().Encode(saltBbigInteger2).ToHex();
//                //Calculate the new contract address
//                contractCalculatedAddress2 = CalculateCreate2AddressMinimalProxy(factoryAddress, saltHex2, defaultForwaderContractAddress);
//                //Let's tranfer some ether, with some extra gas to allow forwarding if the smart contract is deployed (UX problem)
//                var transferEtherReceipt2 = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractCalculatedAddress2, (decimal)0.001, null, 4500000);

//                //Create the clone with the salt to match the address
//                var txnReceipt = await factoryService.CloneForwarderRequestAndWaitForReceiptAsync(defaultForwaderContractAddress, saltBbigInteger);
//                var clonedAdress = txnReceipt.DecodeAllEvents<ForwarderClonedEventDTO>()[0].Event.ClonedAdress;
//                Assert.True(clonedAdress.IsTheSameAddress(contractCalculatedAddress));

//                //Create the clone2 with the salt to match the address
//                var txnReceipt2 = await factoryService.CloneForwarderRequestAndWaitForReceiptAsync(defaultForwaderContractAddress, saltBbigInteger2);
//                var clonedAdress2 = txnReceipt2.DecodeAllEvents<ForwarderClonedEventDTO>()[0].Event.ClonedAdress;
//                Assert.True(clonedAdress2.IsTheSameAddress(contractCalculatedAddress2));

//            }
//            else
//            {
//                //Lets create new invovice to be paid
//                var salts = GetSalts(saltType);
//                var saltByte = salts.Salt;
//                var saltHex = salts.SaltHax;
//                //Calculate the new contract address
//                contractCalculatedAddress = CalculateCreate2AddressMinimalProxy(factoryAddress, saltHex, defaultForwaderContractAddress);
//                //Let's tranfer some ether, with some extra gas to allow forwarding if the smart contract is deployed (UX problem)
//                var transferEtherReceipt = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractCalculatedAddress, (decimal)0.001, null, 4500000);


//                //Lets create new invovice to be paid
//                salts = GetSalts(saltType);
//                var saltByte2 = salts.Salt;
//                var saltHex2 = salts.SaltHax;
//                //Calculate the new contract address
//                contractCalculatedAddress2 = CalculateCreate2AddressMinimalProxy(factoryAddress, saltHex2, defaultForwaderContractAddress);
//                //Let's tranfer some ether, with some extra gas to allow forwarding if the smart contract is deployed (UX problem)
//                var transferEtherReceipt2 = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractCalculatedAddress2, (decimal)0.001, null, 4500000);

//                //Create the clone with the salt to match the address
//                var txnReceipt = await factoryService.CloneForwarder1RequestAndWaitForReceiptAsync(defaultForwaderContractAddress, saltByte);
//                var clonedAdress = txnReceipt.DecodeAllEvents<ForwarderClonedEventDTO>()[0].Event.ClonedAdress;
//                Assert.True(clonedAdress.IsTheSameAddress(contractCalculatedAddress));

//                //Create the clone2 with the salt to match the address
//                var txnReceipt2 = await factoryService.CloneForwarder1RequestAndWaitForReceiptAsync(defaultForwaderContractAddress, saltByte2);
//                var clonedAdress2 = txnReceipt2.DecodeAllEvents<ForwarderClonedEventDTO>()[0].Event.ClonedAdress;
//                Assert.True(clonedAdress2.IsTheSameAddress(contractCalculatedAddress2));
//            }

//            if (isUsedAnotherAccount)
//            {
//                await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(EthereumClientIntegrationFixture.AlternateAccountPublicKey, 1000);
//                factoryService = new ForwarderFactoryService(_ethereumClientIntegrationFixture.GetAlternateWeb3(), factoryDeploymentReceipt.ContractAddress);

//                Func<Task> func = async () =>
//                {
//                    var flushAllReceipt = await factoryService.FlushEtherRequestAndWaitForReceiptAsync(new List<string> { contractCalculatedAddress, contractCalculatedAddress2 });
//                };

//                await func.Should().ThrowAsync<RpcResponseException>("execution reverted: Only owner");
//                return;
//            }
//            else
//            {
//                //Flushing from the factory
//                var flushAllReceipt = await factoryService.FlushEtherRequestAndWaitForReceiptAsync(new List<string> { contractCalculatedAddress, contractCalculatedAddress2 });
//            }
//            //////validate balances... for two forwarders of 0.001 + 0.001
//            var newbalanceDestination = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            Assert.Equal((decimal)0.001 + (decimal)0.001 + balanceDestinationEther, Web3.Web3.Convert.FromWei(newbalanceDestination));
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
//            var destinationAddress = "0x6C547791C3573c2093d81b919350DB1094707011";
//            //Using ropsten infura 
//            //var web3 = _ethereumClientIntegrationFixture.GetInfuraWeb3(InfuraNetwork.Ropsten);
//            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

//            //Getting the current Ether balance of the destination, we are going to transfer 0.001 ether
//            var balanceDestination = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            var balanceDestinationEther = Web3.Web3.Convert.FromWei(balanceDestination);

//            //Deploying first the default forwarder (template for all clones)
//            var defaultForwarderDeploymentReceipt = await ForwarderService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderDeployment());
//            var defaultForwaderContractAddress = defaultForwarderDeploymentReceipt.ContractAddress;
//            var defaultForwarderService = new ForwarderService(web3, defaultForwaderContractAddress);
//            //initialiasing with the destination address
//            await defaultForwarderService.ChangeDestinationRequestAndWaitForReceiptAsync(destinationAddress);
//            var destinationInContract = await defaultForwarderService.DestinationQueryAsync();
//            //validate the destination address has been set correctly
//            Assert.True(destinationInContract.IsTheSameAddress(destinationAddress));

//            //Deploying the factory
//            var factoryDeploymentReceipt = await ForwarderFactoryService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderFactoryDeployment());
//            var factoryAddress = factoryDeploymentReceipt.ContractAddress;
//            var factoryService = new ForwarderFactoryService(web3, factoryDeploymentReceipt.ContractAddress);
//            var addresses = await SendEtherAndCreateClones(50, web3, factoryService, 0.001M, factoryAddress, defaultForwaderContractAddress, saltType);


//            //Flushing from the factory
//            var flushAllReceipt = await factoryService.FlushEtherRequestAndWaitForReceiptAsync(addresses);
//            //check here the cost ^^^
//            var totalEtherTransfered = 0.001M * addresses.Count;

//            var newbalanceDestination = await web3.Eth.GetBalance.SendRequestAsync(destinationAddress);
//            Assert.Equal(totalEtherTransfered + balanceDestinationEther, Web3.Web3.Convert.FromWei(newbalanceDestination));
//        }

//        private async Task<List<string>> SendEtherAndCreateClones(int numberOfClones, Web3.Web3 web3, ForwarderFactoryService factoryService, decimal amount, string factoryAddress, string defaultForwaderContractAddress, SaltType saltType)
//        {

//            var numProcs = Environment.ProcessorCount;
//            var concurrencyLevel = numProcs * 2;
//            var concurrentDictionary = new ConcurrentDictionary<int, string>(concurrencyLevel, numberOfClones * 2);
//            var taskItems = new List<int>();
//            for (var i = 0; i < numberOfClones; i++)
//                taskItems.Add(i);

//            Parallel.ForEach(taskItems, (item, state) =>
//            {
//                var id = item.ToString();
//                var address = SendEtherAndCreateClone(web3, factoryService, amount, id, factoryAddress, defaultForwaderContractAddress, saltType).Result;
//                concurrentDictionary.TryAdd(item, address);
//            });

//            return concurrentDictionary.Values.ToList();
//        }


//        private async Task<string> SendEtherAndCreateClone(Web3.Web3 web3, ForwarderFactoryService factoryService, decimal amount, string saltNumber, string factoryAddress, string defaultForwaderContractAddress, SaltType saltType)
//        {
//            TransactionReceipt txnReceipt;
//            string contractCalculatedAddress;

//            if (saltType == SaltType.Unint)
//            {
//                //Lets create new contract to be paid
//                var salt = BigInteger.Parse(saltNumber); //salt id
//                var saltHex = new IntTypeEncoder().Encode(salt).ToHex();

//                //Calculate the new contract address
//                contractCalculatedAddress = CalculateCreate2AddressMinimalProxy(factoryAddress, saltHex, defaultForwaderContractAddress);

//                //Let's tranfer some ether, with some extra gas to allow forwarding if the smart contract is deployed (UX problem)
//                var transferEtherReceipt = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractCalculatedAddress, amount, null, 4500000);
//                txnReceipt = await factoryService.CloneForwarderRequestAndWaitForReceiptAsync(defaultForwaderContractAddress, salt);

//            }
//            else
//            {
//                var salts = GetSalts(saltType);
//                var salt = salts.Salt;
//                var saltHex = salts.SaltHax;
//                //Calculate the new contract address
//                contractCalculatedAddress = CalculateCreate2AddressMinimalProxy(factoryAddress, saltHex, defaultForwaderContractAddress);

//                //Let's tranfer some ether, with some extra gas to allow forwarding if the smart contract is deployed (UX problem)
//                var transferEtherReceipt = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(contractCalculatedAddress, amount, null, 4500000);
//                txnReceipt = await factoryService.CloneForwarder1RequestAndWaitForReceiptAsync(defaultForwaderContractAddress, salt);

//            }


//            var clonedAdress = txnReceipt.DecodeAllEvents<ForwarderClonedEventDTO>()[0].Event.ClonedAdress;
//            Assert.True(clonedAdress.IsTheSameAddress(contractCalculatedAddress));
//            return contractCalculatedAddress;
//        }

//        #endregion

//        #region

//        [Fact]
//        public async void ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushToken_Salt_Is_Unit()
//        {
//            await ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushToken(SaltType.Unint);
//        }

//        [Fact]
//        public async void ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushToken_Salt_Is_String_Guid()
//        {
//            await ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushToken(SaltType.String_Guid);
//        }

//        [Fact]
//        public async void ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushToken_Salt_Is_String_NBitcoin_RandomUtils()
//        {
//            await ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushToken(SaltType.String_NBitcoin_RandomUtils);
//        }

//        async Task ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushToken(SaltType saltType)
//        {
//            var destinationAddress = "0x6C547791C3573c2093d81b919350DB1094707011";
//            //Using ropsten infura 
//            //var web3 = _ethereumClientIntegrationFixture.GetInfuraWeb3(InfuraNetwork.Ropsten);
//            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

//            //Deploy our custom token
//            var tokenDeploymentReceipt = await ERC20TokenService.DeployContractAndWaitForReceiptAsync(web3,
//                new ERC20TokenDeployment() { DecimalUnits = 18, TokenName = "TST", TokenSymbol = "TST", InitialAmount = Web3.Web3.Convert.ToWei(10000) });
//            var tokenService = new ERC20TokenService(web3, tokenDeploymentReceipt.ContractAddress);

//            //Deploying first the default forwarder (template for all clones)
//            var defaultForwarderDeploymentReceipt = await ForwarderService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderDeployment());
//            var defaultForwaderContractAddress = defaultForwarderDeploymentReceipt.ContractAddress;
//            var defaultForwarderService = new ForwarderService(web3, defaultForwaderContractAddress);
//            //initialiasing with the destination address
//            await defaultForwarderService.ChangeDestinationRequestAndWaitForReceiptAsync(destinationAddress);
//            var destinationInContract = await defaultForwarderService.DestinationQueryAsync();
//            //validate the destination address has been set correctly
//            Assert.True(destinationInContract.IsTheSameAddress(destinationAddress));

//            //Deploying the factory
//            var factoryDeploymentReceipt = await ForwarderFactoryService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderFactoryDeployment());
//            var factoryAddress = factoryDeploymentReceipt.ContractAddress;
//            var factoryService = new ForwarderFactoryService(web3, factoryDeploymentReceipt.ContractAddress);

//            string contractCalculatedAddress;
//            BigInteger saltBigInteger = 0;
//            byte[] saltByte = null;
//            string saltHex;
//            if (saltType == SaltType.Unint)
//            {
//                //Lets create new invovice to be paid
//                saltBigInteger = BigInteger.Parse("12"); //12 our invoice number
//                saltHex = new IntTypeEncoder().Encode(saltBigInteger).ToHex();
//            }
//            else
//            {
//                //Lets create new invovice to be paid
//                var salts = GetSalts(saltType);
//                saltByte = salts.Salt;
//                saltHex = salts.SaltHax;
//            }

//            //Calculate the new contract address
//            contractCalculatedAddress = CalculateCreate2AddressMinimalProxy(factoryAddress, saltHex, defaultForwaderContractAddress);

//            var transferRecipt = await tokenService.TransferRequestAndWaitForReceiptAsync(contractCalculatedAddress, Web3.Web3.Convert.ToWei(0.001));
//            //Check the balance of the adress we sent.. we have not deployed the smart contract so it should be still the same
//            var balanceContract = await tokenService.BalanceOfQueryAsync(contractCalculatedAddress);
//            Assert.Equal((decimal)0.001, Web3.Web3.Convert.FromWei(balanceContract));

//            TransactionReceipt txnReceipt;
//            if (saltType == SaltType.Unint)
//            {
//                //Create the clone with the salt to match the address
//                txnReceipt = await factoryService.CloneForwarderRequestAndWaitForReceiptAsync(defaultForwaderContractAddress, saltBigInteger);
//            }
//            else
//            {
//                //Create the clone with the salt to match the address
//                txnReceipt = await factoryService.CloneForwarder1RequestAndWaitForReceiptAsync(defaultForwaderContractAddress, saltByte);
//            }

//            var clonedAdress = txnReceipt.DecodeAllEvents<ForwarderClonedEventDTO>()[0].Event.ClonedAdress;
//            Assert.True(clonedAdress.IsTheSameAddress(contractCalculatedAddress));

//            //create a service to for cloned forwarder
//            var clonedForwarderService = new ForwarderService(web3, contractCalculatedAddress);
//            var destinationInContractCloned = await clonedForwarderService.DestinationQueryAsync();
//            //validate the destination address is the same
//            Assert.True(destinationInContractCloned.IsTheSameAddress(destinationAddress));

//            //Using flush directly in the cloned contract
//            //call flush to get all the ether transferred to destination address 
//            var flushReceipt = await clonedForwarderService.FlushTokensRequestAndWaitForReceiptAsync(tokenService.ContractHandler.ContractAddress);

//            //validate balances...
//            var newbalanceDestination = await tokenService.BalanceOfQueryAsync(destinationAddress);
//            Assert.Equal((decimal)0.001, Web3.Web3.Convert.FromWei(newbalanceDestination));
//        }

//        #endregion

//        #region

//        [Fact]
//        public async void ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushTokensUsinFactory_Salt_Is_Unit()
//        {
//            await ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushTokensUsinFactory(SaltType.Unint);
//        }

//        [Fact]
//        public async void ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushTokensUsinFactory_Salt_Is_String_Guid()
//        {
//            await ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushTokensUsinFactory(SaltType.String_Guid);
//        }

//        [Fact]
//        public async void ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushTokensUsinFactory_Salt_Is_String_NBitcoin_RandomUtilst()
//        {
//            await ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushTokensUsinFactory(SaltType.String_NBitcoin_RandomUtils);
//        }

//        async Task ShouldDeployForwarder_TransferToken_CloneItUsingFactory_FlushTokensUsinFactory(SaltType saltType)
//        {
//            var destinationAddress = "0x6C547791C3573c2093d81b919350DB1094707011";
//            //Using ropsten infura 
//            //var web3 = _ethereumClientIntegrationFixture.GetInfuraWeb3(InfuraNetwork.Ropsten);
//            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

//            //Deploy our custom token
//            var tokenDeploymentReceipt = await ERC20TokenService.DeployContractAndWaitForReceiptAsync(web3,
//                new ERC20TokenDeployment() { DecimalUnits = 18, TokenName = "TST", TokenSymbol = "TST", InitialAmount = Web3.Web3.Convert.ToWei(10000) });
//            var tokenService = new ERC20TokenService(web3, tokenDeploymentReceipt.ContractAddress);

//            //Deploying first the default forwarder (template for all clones)
//            var defaultForwarderDeploymentReceipt = await ForwarderService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderDeployment());
//            var defaultForwaderContractAddress = defaultForwarderDeploymentReceipt.ContractAddress;
//            var defaultForwarderService = new ForwarderService(web3, defaultForwaderContractAddress);
//            //initialiasing with the destination address
//            await defaultForwarderService.ChangeDestinationRequestAndWaitForReceiptAsync(destinationAddress);
//            var destinationInContract = await defaultForwarderService.DestinationQueryAsync();
//            //validate the destination address has been set correctly
//            Assert.True(destinationInContract.IsTheSameAddress(destinationAddress));

//            //Deploying the factory
//            var factoryDeploymentReceipt = await ForwarderFactoryService.DeployContractAndWaitForReceiptAsync(web3, new ForwarderFactoryDeployment());
//            var factoryAddress = factoryDeploymentReceipt.ContractAddress;
//            var factoryService = new ForwarderFactoryService(web3, factoryDeploymentReceipt.ContractAddress);

//            string saltHex;
//            string saltHex2;
//            BigInteger saltBigInteger = 0;
//            BigInteger saltBigInteger2 = 0;
//            byte[] saltByte = null;
//            byte[] saltByte2 = null;

//            if (saltType == SaltType.Unint)
//            {
//                //Lets create new salt
//                saltBigInteger = BigInteger.Parse("12"); //12
//                saltHex = new IntTypeEncoder().Encode(saltBigInteger).ToHex();

//                //Lets create new salt for another 
//                saltBigInteger2 = BigInteger.Parse("13"); //13
//                saltHex2 = new IntTypeEncoder().Encode(saltBigInteger2).ToHex();
//            }
//            else
//            {
//                var salts = GetSalts(saltType);
//                saltByte = salts.Salt;
//                saltHex = salts.SaltHax;

//                salts = GetSalts(saltType);
//                saltByte2 = salts.Salt;
//                saltHex2 = salts.SaltHax;
//            }


//            //Calculate the new contract address
//            var contractCalculatedAddress = CalculateCreate2AddressMinimalProxy(factoryAddress, saltHex, defaultForwaderContractAddress);

//            //Calculate the new contract address
//            var contractCalculatedAddress2 = CalculateCreate2AddressMinimalProxy(factoryAddress, saltHex2, defaultForwaderContractAddress);


//            var transferRecipt = await tokenService.TransferRequestAndWaitForReceiptAsync(contractCalculatedAddress, Web3.Web3.Convert.ToWei(0.001));
//            //Check the balance of the adress we sent.. we have not deployed the smart contract so it should be still the same
//            var balanceContract = await tokenService.BalanceOfQueryAsync(contractCalculatedAddress);
//            Assert.Equal((decimal)0.001, Web3.Web3.Convert.FromWei(balanceContract));

//            var transferReceipt2 = await tokenService.TransferRequestAndWaitForReceiptAsync(contractCalculatedAddress2, Web3.Web3.Convert.ToWei(0.001));

//            if (saltType == SaltType.Unint)
//            {
//                //Create the clone with the salt to match the address
//                var txnReceipt = await factoryService.CloneForwarderRequestAndWaitForReceiptAsync(defaultForwaderContractAddress, saltBigInteger);
//                var clonedAdress = txnReceipt.DecodeAllEvents<ForwarderClonedEventDTO>()[0].Event.ClonedAdress;
//                Assert.True(clonedAdress.IsTheSameAddress(contractCalculatedAddress));


//                var txnReceipt2 = await factoryService.CloneForwarderRequestAndWaitForReceiptAsync(defaultForwaderContractAddress, saltBigInteger2);
//                var clonedAdress2 = txnReceipt2.DecodeAllEvents<ForwarderClonedEventDTO>()[0].Event.ClonedAdress;
//                Assert.True(clonedAdress2.IsTheSameAddress(contractCalculatedAddress2));
//            }
//            else
//            {
//                //Create the clone with the salt to match the address
//                var txnReceipt = await factoryService.CloneForwarder1RequestAndWaitForReceiptAsync(defaultForwaderContractAddress, saltByte);
//                var clonedAdress = txnReceipt.DecodeAllEvents<ForwarderClonedEventDTO>()[0].Event.ClonedAdress;
//                Assert.True(clonedAdress.IsTheSameAddress(contractCalculatedAddress));

//                var txnReceipt2 = await factoryService.CloneForwarder1RequestAndWaitForReceiptAsync(defaultForwaderContractAddress, saltByte2);
//                var clonedAdress2 = txnReceipt2.DecodeAllEvents<ForwarderClonedEventDTO>()[0].Event.ClonedAdress;
//                Assert.True(clonedAdress2.IsTheSameAddress(contractCalculatedAddress2));
//            }

//            //Flushing from the factory
//            var flushAllReceipt = await factoryService.FlushTokensRequestAndWaitForReceiptAsync(new List<string> { contractCalculatedAddress, contractCalculatedAddress2 }, tokenService.ContractHandler.ContractAddress);

//            //////validate balances... for two forwarders of 0.001 + 0.001
//            var newbalanceDestination = await tokenService.BalanceOfQueryAsync(destinationAddress);
//            Assert.Equal((decimal)0.001 + (decimal)0.001, Web3.Web3.Convert.FromWei(newbalanceDestination));
//        }

//        #endregion

//    }
//}