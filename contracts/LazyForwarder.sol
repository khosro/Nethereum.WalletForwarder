// https://eips.ethereum.org/EIPS/eip-20
// SPDX-License-Identifier: MIT
pragma solidity >=0.5.0 <0.8.0;
import "./Erc20.sol";
/**
 * Contract that will forward any incoming Ether to the creator of the contract
 */
contract LazyForwarder {
  // Address to which any funds sent to this contract will be forwarded
  
  address payable private destination;
  address   private caller;
  
  bool inititalised = false;

  event ForwarderDeposited(address from, uint value, bytes data);
  event TokensFlushed(address forwarderAddress, uint value, address tokenContractAddress);

  /**
   * Create the contract, and sets the destination address to that of the creator
   * set initialised true for the default forwarder on normal contract deployment
   */
  constructor() {
    destination = msg.sender;
    inititalised = true;
  }

 modifier onlyDestinationOrOwner {
    if (msg.sender != destination && msg.sender != caller) {
      revert("Only destination and owner");
    }
    _;
  }

  receive() external payable {
     emit ForwarderDeposited(msg.sender, msg.value, msg.data);
  }

  //init on create2
  function init(address payable des,address call1) public {
      if(!inititalised){
         caller = call1;
          destination = des;
          inititalised = true;
      }
  }
 
  //flush the tokens
  function flushTokens(address tokenContractAddress) public onlyDestinationOrOwner {
    IERC20 instance = IERC20(tokenContractAddress);
    uint256 forwarderBalance = instance.balanceOf(address(this));
    if (forwarderBalance == 0) {
      revert();
    }
    if (!instance.transfer(destination, forwarderBalance)) {
      revert();
    }
    emit TokensFlushed(address(this), forwarderBalance, tokenContractAddress);
  }

  function flush() payable public onlyDestinationOrOwner {
    address payable thisContract = address(this);
     destination.transfer(thisContract.balance);
  }
  
}

