﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using WSBC.DiscordBot.Explorer;

namespace WSBC.DiscordBot.Discord.Commands
{
    public class CoinCheckCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ICoinDataProvider _dataProvider;
        private readonly ICoinDataEmbedBuilder _embedBuilder;
        private readonly IExplorerDataClient _explorerClient;
        private readonly ILogger _log;

        public CoinCheckCommands(ICoinDataProvider dataProvider, ICoinDataEmbedBuilder embedBuilder, IExplorerDataClient explorerClient,
            ILogger<CoinCheckCommands> log)
        {
            this._dataProvider = dataProvider;
            this._embedBuilder = embedBuilder;
            this._explorerClient = explorerClient;
            this._log = log;
        }

        [Command("coin")]
        [Summary("Shows current currency data")]
        public async Task CoinDataAsync()
        {
            Task SendErrorAsync()
                => base.Context.Channel.SendMessageAsync($"\u274C Couldn't retrieve coin data.");

            using IDisposable logScope = this._log.BeginCommandScope(base.Context, this);

            CoinData data;
            try
            {
                data = await this._dataProvider.GetDataAsync().ConfigureAwait(false);
                if (data == null)
                {
                    await SendErrorAsync().ConfigureAwait(false);
                    return;
                }
            }
            catch
            {
                await SendErrorAsync().ConfigureAwait(false);
                throw;
            }
            Embed embed = this._embedBuilder.Build(data, base.Context.Message);
            await base.Context.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("block")]
        [Summary("Gets block data")]
        public async Task BlockDataAsync(string block)
        {
            Task SendErrorAsync()
                => base.Context.Channel.SendMessageAsync($"\u274C Couldn't retrieve data about block `{block}`.");

            using IDisposable logScope = this._log.BeginCommandScope(base.Context, this);

            ExplorerBlockData data;
            try
            {
                data = await this._explorerClient.GetBlockDataAsync(block).ConfigureAwait(false);
                if (data == null)
                {
                    await SendErrorAsync().ConfigureAwait(false);
                    return;
                }
            }
            catch 
            {
                await SendErrorAsync().ConfigureAwait(false);
                throw;
            }

            Embed embed = this._embedBuilder.Build(data, base.Context.Message);
            await base.Context.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("transaction")]
        [Alias("tx")]
        [Summary("Gets transaction data")]
        public async Task TransactionDataAsync(string hash)
        {
            Task SendErrorAsync()
                => base.Context.Channel.SendMessageAsync($"\u274C Couldn't retrieve data about transaction `{hash}`.");

            using IDisposable logScope = this._log.BeginCommandScope(base.Context, this);

            ExplorerTransactionData data;
            try
            {
                data = await this._explorerClient.GetTransactionDataAsync(hash).ConfigureAwait(false);
                if (data == null)
                {
                    await SendErrorAsync().ConfigureAwait(false);
                    return;
                }
            }
            catch
            {
                await SendErrorAsync().ConfigureAwait(false);
                throw;
            }

            Embed embed = this._embedBuilder.Build(data, base.Context.Message);
            await base.Context.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }
    }
}
