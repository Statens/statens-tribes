using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Statens.Tribes.App.Domain.Infrastructure;
using Statens.Tribes.App.Domain.Interfaces;
using Statens.Tribes.App.Domain.Model;

namespace Statens.Tribes.App.Domain.Services
{
    public class TribeService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepositoryOfType<Tribe> _tribeRepository;
        private readonly IRepositoryOfType<TribeMembershipList> _membershipRepository;
        private readonly ILogger<TribeService> _logger;

        public TribeService(IRepositoryOfType<Tribe> tribeRepository,
            IRepositoryOfType<TribeMembershipList> membershipRepository,
            ILogger<TribeService> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _tribeRepository = tribeRepository;
            _membershipRepository = membershipRepository;
            _logger = logger;

            _logger.LogTrace("Initializing");
        }

        public Tribe CreateTribe(string id, string name, TribeType type)
        {
            _logger.LogDebug("CreateTribe");
            return new Tribe
            {
                Id = id,
                Name = name,
                Type = type,
                Members = new List<TribeMember>()
            };
        }
    }
}