﻿using System.Linq;
using System.Threading.Tasks;
using HashBus.Application.Events;
using HashBus.Twitter.Events;
using LiteGuard;
using NServiceBus;
using ColoredConsole;

namespace HashBus.Application
{
    public class HashtagTweetedHandler : IHandleMessages<HashtagTweeted>
    {
        private ISendOnlyBus bus;

        public HashtagTweetedHandler(ISendOnlyBus bus)
        {
            Guard.AgainstNullArgument("bus", bus);

            this.bus = bus;
        }

        public async Task Handle(HashtagTweeted message)
        {
            foreach (var mentionMessage in message.TweetUserMentions.Select(userMention => new UserMentionedWithHashtag
            {
                Hashtag = message.Hashtag,
                TweetId = message.TweetId,
                TweetCreatedAt = message.TweetCreatedAt,
                TweetCreatedById = message.TweetCreatedById,
                TweetCreatedByIdStr = message.TweetCreatedByIdStr,
                TweetCreatedByName = message.TweetCreatedByName,
                TweetCreatedByScreenName = message.TweetCreatedByScreenName,
                TweetIsRetweet = message.TweetIsRetweet,
                TweetText = message.TweetText,
                UserMentionId = userMention.Id,
                UserMentionIdStr = userMention.IdStr,
                UserMentionName = userMention.Name,
                UserMentionScreenName = userMention.ScreenName,
            }))
            {
                ColorConsole.WriteLine(
                    $"{mentionMessage.TweetCreatedAt} ".DarkCyan(),
                    $"#{message.Hashtag} ".Cyan(),
                    mentionMessage.TweetIsRetweet ? "retweet mentioning ".DarkGreen() : "tweet mentioning ".Green(),
                    $"{mentionMessage.UserMentionName} ".Yellow(),
                    $"@{mentionMessage.UserMentionScreenName}".DarkYellow());

                await bus.PublishAsync(mentionMessage);
            }
        }
    }
}