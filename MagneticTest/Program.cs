// See https://aka.ms/new-console-template for more information

using System.Linq.Expressions;

using XRPL.MagneticService;
using XRPL.MagneticService.Entities;

Console.WriteLine("Hello, World!");

var client = new MagneticClient(true, null);

var dices = await client.GetDiceHistory(100);
var dices2 = await client.GetDiceHistory(1,50,"XRP",null);
var dices3 = await client.GetFullDiceHistory("xrp","rLiooJRSKeiNfRJcDBUhu4rcjQjGLWqa4p");


var settrings = await client.GetDiceSettings();
var check = await client.CheckDice(dices.Data[0].Token, dices.Data[0].Wallet);

var wallet_nfts = await client.GetNFTs("rLiooJRSKeiNfRJcDBUhu4rcjQjGLWqa4p");
var power = client.GetDiceNFTsPower(wallet_nfts, settrings);

Console.ReadLine();
