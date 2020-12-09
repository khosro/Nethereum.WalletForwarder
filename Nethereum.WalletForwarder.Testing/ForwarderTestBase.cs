using NBitcoin;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using Nethereum.XUnitEthereumClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nethereum.Forwarder.IntegrationTests
{
    public class ForwarderTestBase
    {
        protected readonly EthereumClientIntegrationFixture _ethereumClientIntegrationFixture;

        public ForwarderTestBase(EthereumClientIntegrationFixture ethereumClientIntegrationFixture)
        {
            _ethereumClientIntegrationFixture = ethereumClientIntegrationFixture;
        }

        protected (byte[] Salt, string SaltHax) GetSalts(SaltType saltType)
        {
            if (saltType == SaltType.String_Guid)
            {
                var salt = Encoding.ASCII.GetBytes(Guid.NewGuid().ToString().Replace("-", ""));
                var saltHex = salt.ToHex();

                return (salt, saltHex);
            }
            else if (saltType == SaltType.String_NBitcoin_RandomUtils)
            {
                var salt = RandomUtils.GetBytes(32);
                var saltHex = salt.ToHex();

                return (salt, saltHex);
            }
            else
            {
                throw new NotSupportedException($"Wrong {saltType}");
            }

            /*
            var salt = new BigInteger(RandomUtils.GetBytes(32)); //salt id
            if (salt < 0) // Because maybe we have negative salt and positive salt at the same time then, we can not use it.
            {
                salt = -1 * salt;
            }
             var saltHex = new IntTypeEncoder().Encode(salt).ToHex();
            */
        }

        protected async Task TransferToDestination(Web3.Web3 web3)
        {
            await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(EthereumClientIntegrationFixture.DestinationPublicKey, 100, null, 4500000);
        }

        protected async Task TransferToAlternateAccount(Web3.Web3 web3)
        {
            await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(EthereumClientIntegrationFixture.AlternateAccountPublicKey, 1000);
        }

        //extracted from latest Nethereum Util
        public static string CalculateCreate2AddressMinimalProxy(string address, string saltHex, string deploymentAddress)
        {
            if (string.IsNullOrEmpty(deploymentAddress))
            {
                throw new System.ArgumentException($"'{nameof(deploymentAddress)}' cannot be null or empty.", nameof(deploymentAddress));
            }

            var bytecode = "3d602d80600a3d3981f3363d3d373d3d3d363d73" + deploymentAddress.RemoveHexPrefix() + "5af43d82803e903d91602b57fd5bf3";
            return CalculateCreate2Address(address, saltHex, bytecode);
        }

        //extracted from latest Nethereum Util
        public static string CalculateCreate2Address(string address, string saltHex, string byteCodeHex)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new System.ArgumentException($"'{nameof(address)}' cannot be null or empty.", nameof(address));
            }

            if (string.IsNullOrEmpty(saltHex))
            {
                throw new System.ArgumentException($"'{nameof(saltHex)}' cannot be null or empty.", nameof(saltHex));
            }

            if (saltHex.EnsureHexPrefix().Length != 66)
            {
                throw new System.ArgumentException($"'{nameof(saltHex)}' needs to be 32 bytes", nameof(saltHex));
            }

            var sha3 = new Sha3Keccack();
            return sha3.CalculateHashFromHex("0xff", address, saltHex, sha3.CalculateHashFromHex(byteCodeHex)).Substring(24).ConvertToEthereumChecksumAddress();
        }

    }
}
