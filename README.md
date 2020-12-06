# Nethereum.WalletForwarder

Example of Wallet Forwarder based on Coinbase, Bitgo and primozkocevar implementations. Detailed explanation to follow
 
For coinbase descrition refer to `https://blog.coinbase.com/usdc-payment-processing-in-coinbase-commerce-b1af1c82fb0`

---

## Run tests

In odrder to run test, set values in `Nethereum.WalletForwarder.Testing\appsettings.test.json` or set those values in environment variables.

For example in order to run test in `geth` test chain set the following values :

```

setx EthereumTestSettings_GethPath  "../../../../../testchain/clique/geth"

setx EthereumTestSettings_Client  "Geth"

```

And then download the files from [geth clique](https://github.com/Nethereum/Nethereum/tree/master/testchain/clique) and put them in a folder named `testchain` and locate it beside of `Nethereum.WalletForwarder` parent folder. such as the following :

```
     |Nethereum.WalletForwarder
                               |testchain
                               |Nethereum.WalletForwarder
                               |Nethereum.WalletForwarder.Testing
                               |.....(Others)
```     

---