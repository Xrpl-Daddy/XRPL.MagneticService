// See https://aka.ms/new-console-template for more information

using System.Linq.Expressions;

using XRPL.MagneticService;
using XRPL.MagneticService.Entities;

Console.WriteLine("Hello, World!");

var client = new MagneticClient(true, null);


var block_info = await client.GetBlockInfo();
var mag_pools = await client.GetMagPools();
var sponsor_pools = await client.GetSponsorPools();

var mag_pools_for_account = await client.GetMagPools("rLiooJRSKeiNfRJcDBUhu4rcjQjGLWqa4p");
var sponsor_pools_for_account = await client.GetSponsorPools("rLiooJRSKeiNfRJcDBUhu4rcjQjGLWqa4p");


var dices = await client.GetDiceHistory(100);
var dices2 = await client.GetDiceHistory(1,50,"XRP",null);
var dices3 = await client.GetFullDiceHistory("xrp","rLiooJRSKeiNfRJcDBUhu4rcjQjGLWqa4p");
var dices4 = await client.GetFullDiceHistory();
var stats = await client.GetDiceStats("rLiooJRSKeiNfRJcDBUhu4rcjQjGLWqa4p","xrp");



var settrings = await client.GetDiceSettings();
var check = await client.CheckDice(dices.Data[0].Token, dices.Data[0].Wallet);

var wallet_nfts = await client.GetNFTs("rLiooJRSKeiNfRJcDBUhu4rcjQjGLWqa4p");
var power = client.GetDiceNFTsPower(wallet_nfts, settrings);

Console.ReadLine();
