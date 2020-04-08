﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using SnaffCore.Concurrency;
using SnaffCore.ShareFind;
using SnaffCore.ShareScan;
using Config = SnaffCore.Config.Config;

namespace Classifiers
{
    public partial class Classifier
    {
        public ShareFinder.ShareResult ClassifyShare(string share, Config config)
        {
            // check if it matches
            if (SimpleMatch(share))
            {
                bool sendToNextScope = false;
                bool sendToMq = false;
                // if it does, see what we're gonna do with it
                switch (MatchAction)
                {
                    case MatchAction.Discard:
                        return null;
                        break;
                    case MatchAction.Snaffle:
                        if (IsShareReadable(share, config))
                        {
                            sendToMq = true;
                        }
                        break;
                    case MatchAction.SendToNextScope:
                        if (IsShareReadable(share, config))
                        {
                            sendToMq = true;
                            sendToNextScope = true;
                        }
                        break;
                    default:
                        return null;
                        break;
                }

                ShareFinder.ShareResult shareResult = new ShareFinder.ShareResult()
                {
                    Listable = true,
                    SharePath = share,
                    Snaffle = sendToMq,
                    ScanShare = sendToNextScope
                };
                return shareResult;
            }
            else return null;
        }

        internal bool IsShareReadable(string share, Config config)
        {
            try
            {
                string[] files = Directory.GetFiles(share);
                return true;
            }
            catch (Exception e)
            {
                config.Mq.Trace(e.ToString());
            }
            return false;
        }
    }
}