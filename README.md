# KeepIndexer
Keep Indexer is the first Indexer application that return various data about the Keep network (https://keep.network/) in JSON format (for example, TDT_ID) so that application could be used as a data source by other applications like DApps.

The main purpose of this API is to use it in the Redeem operation in the tBTC DApp (https://github.com/keep-network/tbtc-dapp).

# API reference
 - GET {host:port}/api/op/tdt_id?lot={amount}&token={token} // to get random TDT_ID by lot size and token symbol
 - GET {host:port}/api/op/txlist?sender={sender_address} // to get a list of operations by the senders's address
 - GET {host:port}/api/op/list // get list of all operations

## Sample requests:
 - http://62.171.139.205:8080/api/op/tdt_id?lot=1&token=TBTC
 - http://62.171.139.205:8080/api/op/list


**Full application description**: https://medium.com/@aeantonov/building-keep-network-indexer-tbtc-dapp-improvement-ebdfdbd5994f?source=friends_link&sk=2b5b3f6da6705b0cdeba90b191d0dc4e

