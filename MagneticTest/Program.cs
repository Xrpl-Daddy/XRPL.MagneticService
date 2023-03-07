// See https://aka.ms/new-console-template for more information

using XRPL.MagneticService;

Console.WriteLine("Hello, World!");

var client = new MagneticClient(true, null);

var dices = await client.GetDiceHistory(100);
var settrings = await client.GetDiceSettings();
var check = await client.CheckDice(dices.Data[0].Token, dices.Data[0].Wallet);

var wallet_nfts = await client.GetNFTs("rLiooJRSKeiNfRJcDBUhu4rcjQjGLWqa4p");
var power = await client.GetDiceNFTsPower(wallet_nfts, settrings);

Console.ReadLine();
