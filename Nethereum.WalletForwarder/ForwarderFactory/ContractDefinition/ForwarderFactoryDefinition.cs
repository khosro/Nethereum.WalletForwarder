using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace Nethereum.WalletForwarder.Contracts.ForwarderFactory.ContractDefinition
{


    public partial class ForwarderFactoryDeployment : ForwarderFactoryDeploymentBase
    {
        public ForwarderFactoryDeployment() : base(BYTECODE) { }
        public ForwarderFactoryDeployment(string byteCode) : base(byteCode) { }
    }

    public class ForwarderFactoryDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "608060405234801561001057600080fd5b506104c9806100206000396000f3fe608060405234801561001057600080fd5b506004361061004c5760003560e01c80632cbd9ea2146100515780636ac427111461005157806396909a7914610099578063bcebbcd41461013e575b600080fd5b61007d6004803603604081101561006757600080fd5b506001600160a01b0381351690602001356101ec565b604080516001600160a01b039092168252519081900360200190f35b61013c600480360360208110156100af57600080fd5b8101906020810181356401000000008111156100ca57600080fd5b8201836020820111156100dc57600080fd5b803590602001918460208302840111640100000000831117156100fe57600080fd5b91908080602002602001604051908101604052809392919081815260200183836020028082843760009201919091525092955061031a945050505050565b005b61013c6004803603604081101561015457600080fd5b81019060208101813564010000000081111561016f57600080fd5b82018360208201111561018157600080fd5b803590602001918460208302840111640100000000831117156101a357600080fd5b919080806020026020016040519081016040528093929190818152602001838360200280828437600092019190915250929550505090356001600160a01b031691506103a29050565b6000806101f9848461043f565b90506000849050819250826001600160a01b03166319ab453c826001600160a01b031663b269681d6040518163ffffffff1660e01b815260040160206040518083038186803b15801561024b57600080fd5b505afa15801561025f573d6000803e3d6000fd5b505050506040513d602081101561027557600080fd5b5051604080516001600160e01b031960e085901b1681526001600160a01b03909216600483015251602480830192600092919082900301818387803b1580156102bd57600080fd5b505af11580156102d1573d6000803e3d6000fd5b5050604080516001600160a01b038616815290517f5dd8f89d9637eb98e980512c69ed8152bd1abced4c6785ceeb9f7628bd42e8099350908190036020019150a1505092915050565b60005b815181101561039e57600082828151811061033457fe5b60200260200101519050806001600160a01b0316636b9f96ea6040518163ffffffff1660e01b8152600401600060405180830381600087803b15801561037957600080fd5b505af115801561038d573d6000803e3d6000fd5b50506001909301925061031d915050565b5050565b60005b825181101561043a5760008382815181106103bc57fe5b60200260200101519050806001600160a01b0316633ef13367846040518263ffffffff1660e01b815260040180826001600160a01b03168152602001915050600060405180830381600087803b15801561041557600080fd5b505af1158015610429573d6000803e3d6000fd5b5050600190930192506103a5915050565b505050565b6000808360601b9050604051733d602d80600a3d3981f3363d3d373d3d3d363d7360601b81528160148201526e5af43d82803e903d91602b57fd5bf360881b6028820152836037826000f59594505050505056fea2646970667358221220f9bcc88083002ee660c7ca71c22a89b6b5e8dfd6d5f49cd67c2c587e409272fe64736f6c63430007000033";
        public ForwarderFactoryDeploymentBase() : base(BYTECODE) { }
        public ForwarderFactoryDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class CloneForwarderFunction : CloneForwarderFunctionBase { }
    public partial class CloneForwarder1Function : CloneForwarder1FunctionBase { }

    [Function("cloneForwarder", "address")]
    public class CloneForwarderFunctionBase : FunctionMessage
    {
        [Parameter("address", "forwarder", 1)]
        public virtual string Forwarder { get; set; }
        [Parameter("uint256", "salt", 2)]
        public virtual BigInteger Salt { get; set; }
    }

    [Function("cloneForwarder1", "address")]
    public class CloneForwarder1FunctionBase : FunctionMessage
    {
        [Parameter("address", "forwarder", 1)]
        public virtual string Forwarder { get; set; }

        [Parameter("bytes32", "salt", 2)]
        public byte[] Salt { get; set; }
    }

    public partial class FlushEtherFunction : FlushEtherFunctionBase { }

    [Function("flushEther")]
    public class FlushEtherFunctionBase : FunctionMessage
    {
        [Parameter("address[]", "forwarders", 1)]
        public virtual List<string> Forwarders { get; set; }
    }

    public partial class FlushTokensFunction : FlushTokensFunctionBase { }

    [Function("flushTokens")]
    public class FlushTokensFunctionBase : FunctionMessage
    {
        [Parameter("address[]", "forwarders", 1)]
        public virtual List<string> Forwarders { get; set; }
        [Parameter("address", "tokenAddres", 2)]
        public virtual string TokenAddres { get; set; }
    }

    public partial class ForwarderClonedEventDTO : ForwarderClonedEventDTOBase { }

    [Event("ForwarderCloned")]
    public class ForwarderClonedEventDTOBase : IEventDTO
    {
        [Parameter("address", "clonedAdress", 1, false )]
        public virtual string ClonedAdress { get; set; }
    }






}
