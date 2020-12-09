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

namespace Nethereum.WalletForwarder.Contracts.Forwarder.ContractDefinition
{
    public partial class LazyForwarderDeployment : LazyForwarderDeploymentBase
    {
        public LazyForwarderDeployment() : base(BYTECODE) { }
        public LazyForwarderDeployment(string byteCode) : base(byteCode) { }
    }

    public class LazyForwarderDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "60806040526001805460ff60a01b1916905534801561001d57600080fd5b50600080546001600160a01b031916331790556001805460ff60a01b1916600160a01b179055610449806100526000396000f3fe6080604052600436106100385760003560e01c80633ef13367146100be5780636b9f96ea146100f3578063f09a4016146100fb576100b9565b366100b9577f69b31548dea9b3b707b4dff357d326e3e9348b24e7a6080a218a6edeeec48f9b333460003660405180856001600160a01b03168152602001848152602001806020018281038252848482818152602001925080828437600083820152604051601f909101601f191690920182900397509095505050505050a1005b600080fd5b3480156100ca57600080fd5b506100f1600480360360208110156100e157600080fd5b50356001600160a01b0316610136565b005b6100f161030a565b34801561010757600080fd5b506100f16004803603604081101561011e57600080fd5b506001600160a01b03813581169160200135166103c3565b6000546001600160a01b0316331480159061015c57506001546001600160a01b03163314155b156101ae576040805162461bcd60e51b815260206004820152601a60248201527f4f6e6c792064657374696e6174696f6e20616e64206f776e6572000000000000604482015290519081900360640190fd5b604080516370a0823160e01b8152306004820152905182916000916001600160a01b038416916370a08231916024808301926020929190829003018186803b1580156101f957600080fd5b505afa15801561020d573d6000803e3d6000fd5b505050506040513d602081101561022357600080fd5b505190508061023157600080fd5b600080546040805163a9059cbb60e01b81526001600160a01b0392831660048201526024810185905290519185169263a9059cbb926044808401936020939083900390910190829087803b15801561028857600080fd5b505af115801561029c573d6000803e3d6000fd5b505050506040513d60208110156102b257600080fd5b50516102bd57600080fd5b60408051308152602081018390526001600160a01b0385168183015290517fb4bdccee2343c0b5e592d459c20eb1fa451c96bf88fb685a11aecda6b4ec76b19181900360600190a1505050565b6000546001600160a01b0316331480159061033057506001546001600160a01b03163314155b15610382576040805162461bcd60e51b815260206004820152601a60248201527f4f6e6c792064657374696e6174696f6e20616e64206f776e6572000000000000604482015290519081900360640190fd5b6000805460405130926001600160a01b0390921691833180156108fc02929091818181858888f193505050501580156103bf573d6000803e3d6000fd5b5050565b600154600160a01b900460ff166103bf5760018054600080546001600160a01b038681166001600160a01b03199283161790925560ff60a01b1991851692169190911716600160a01b179055505056fea26469706673582212205ba7197707e55142bddb95f851c47c973ffabddd932e7f93f944709daf6fb8ee64736f6c63430007000033";

        public LazyForwarderDeploymentBase() : base(BYTECODE) { }
        public LazyForwarderDeploymentBase(string byteCode) : base(byteCode) { }

    }

    //public partial class LazyChangeDestinationFunction : LazyChangeDestinationFunctionBase { }

    //[Function("changeDestination")]
    //public class LazyChangeDestinationFunctionBase : FunctionMessage
    //{
    //    [Parameter("address", "newDestination", 1)]
    //    public virtual string NewDestination { get; set; }
    //}

    public partial class LazyDestinationFunction : LazyDestinationFunctionBase { }

    [Function("destination", "address")]
    public class LazyDestinationFunctionBase : FunctionMessage
    { }

    public partial class LazyFlushFunction : LazyFlushFunctionBase { }

    [Function("flush")]
    public class LazyFlushFunctionBase : FunctionMessage
    { }

    public partial class LazyFlushTokensFunction : LazyFlushTokensFunctionBase { }

    [Function("flushTokens")]
    public class LazyFlushTokensFunctionBase : FunctionMessage
    {
        [Parameter("address", "tokenContractAddress", 1)]
        public virtual string TokenContractAddress { get; set; }
    }

    public partial class LazyInitFunction : LazyInitFunctionBase { }

    [Function("init")]
    public class LazyInitFunctionBase : FunctionMessage
    {
        [Parameter("address", "newDestination", 1)]
        public virtual string NewDestination { get; set; }
    }

    public partial class LazyWithdrawFunction : LazyWithdrawFunctionBase { }

    [Function("withdraw")]
    public class LazyWithdrawFunctionBase : FunctionMessage
    { }

    public partial class LazyForwarderDepositedEventDTO : LazyForwarderDepositedEventDTOBase { }

    [Event("ForwarderDeposited")]
    public class LazyForwarderDepositedEventDTOBase : IEventDTO
    {
        [Parameter("address", "from", 1, false)]
        public virtual string From { get; set; }
        [Parameter("uint256", "value", 2, false)]
        public virtual BigInteger Value { get; set; }
        [Parameter("bytes", "data", 3, false)]
        public virtual byte[] Data { get; set; }
    }

    public partial class LazyTokensFlushedEventDTO : LazyTokensFlushedEventDTOBase { }

    [Event("TokensFlushed")]
    public class LazyTokensFlushedEventDTOBase : IEventDTO
    {
        [Parameter("address", "forwarderAddress", 1, false)]
        public virtual string ForwarderAddress { get; set; }
        [Parameter("uint256", "value", 2, false)]
        public virtual BigInteger Value { get; set; }
        [Parameter("address", "tokenContractAddress", 3, false)]
        public virtual string TokenContractAddress { get; set; }
    }



    public partial class LazyDestinationOutputDTO : LazyDestinationOutputDTOBase { }

    [FunctionOutput]
    public class LazyDestinationOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

}
