using System.Net;
using System.Net.Mail;
using WebApplication1.Dto;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class LotteryService: ILotteryService
    {
        private readonly ILotteryRepository lotteryRepository;
        private readonly IGiftRepository giftRepository;
        private readonly ILogger<LotteryService> logger;
        public LotteryService(ILotteryRepository lotteryRepository,
            IGiftRepository giftRepository,
            ILogger<LotteryService> logger)
        {
            this.lotteryRepository = lotteryRepository;
            this.giftRepository = giftRepository;
            this.logger = logger;
        }

        public async Task<int> GetAllRevenue()
        {
            logger.LogInformation("GetAllRevenue: starting retrieval of total revenue.");
            try
            {
                var revenue = await lotteryRepository.GetAllRevenue();
                logger.LogInformation("GetAllRevenue: retrieved revenue = {Revenue}.", revenue);
                return revenue;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllRevenue: error while retrieving revenue.");
                throw;
            }
        }

        public async Task<List<DtoLottery>> GetAllWinnersAsync()
        {
            logger.LogInformation("GetAllWinnersAsync: retrieving winners.");
            try
            {
                var winners = await lotteryRepository.GetAllWinnersAsync();
                if (winners == null)
                {
                    logger.LogWarning("GetAllWinnersAsync: winners result is null.");
                    throw new KeyNotFoundException("Error getting winners");
                }
                logger.LogInformation("GetAllWinnersAsync: found {Count} winners.", winners.Count);
                return winners.Select(MapToResponseDto).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllWinnersAsync: unexpected error.");
                throw;
            }
        }

        public async Task<bool> LotteryAsync()
        {
            logger.LogInformation("LotteryAsync: starting lottery run.");
            try
            {
                var gifts = await giftRepository.GetGiftsAsync();
                if (gifts == null || gifts.Count == 0)
                {
                    logger.LogWarning("LotteryAsync: no gifts found for lottery.");
                    throw new KeyNotFoundException("No gifts found for lottery");
                }

                var random = new Random();
                foreach (var gift in gifts)
                {
                    logger.LogInformation("LotteryAsync: processing gift {GiftId} - {GiftName}.", gift.Id, gift.Name);

                    var users = await lotteryRepository.LotteryAsync(gift);
                    if (users == null)
                    {
                        logger.LogWarning("LotteryAsync: no users found for gift {GiftId}.", gift.Id);
                        throw new KeyNotFoundException($"No users found for gift {gift.Name}");
                    }

                    if (users.Any())
                    {
                        var winner = users[random.Next(users.Count)];
                        logger.LogInformation("LotteryAsync: selected potential winner {UserId} for gift {GiftId}.", winner?.Id, gift.Id);

                        if (winner == null)
                        {
                            logger.LogWarning("LotteryAsync: selected winner was null for gift {GiftId}.", gift.Id);
                            return false;
                        }

                        var l = await lotteryRepository.CreateLottery(gift.Id, winner.Id);
                        if (l == null)
                        {
                            logger.LogError("LotteryAsync: failed to create lottery record for gift {GiftId} and user {UserId}.", gift.Id, winner.Id);
                            throw new Exception("Error creating lottery record");
                        }

                        logger.LogInformation("LotteryAsync: lottery record created for gift {GiftId} and user {UserId}. Sending email...", gift.Id, winner.Id);
                        var emailSent = await SendingEmail(winner, gift);
                        logger.LogInformation("LotteryAsync: email send result for user {UserId}: {Result}.", winner.Id, emailSent);
                    }
                    else
                    {
                        logger.LogInformation("LotteryAsync: no eligible users for gift {GiftId}.", gift.Id);
                    }
                }

                logger.LogInformation("LotteryAsync: completed lottery run successfully.");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LotteryAsync: unhandled exception during lottery run.");
                throw;
            }
        }

        public async Task<bool> SendingEmail(User user, Gift gift)
        {
            logger.LogInformation("SendingEmail: preparing to send email to user {UserId} ({Email}) for gift {GiftId} ({GiftName}).",
                user?.Id, user?.Email, gift?.Id, gift?.Name);

            if (user == null || gift == null)
            {
                logger.LogWarning("SendingEmail: user or gift is null.");
                throw new ArgumentNullException("User or Gift is null");
            }

            var toEmail = user.Email;
            var message = new MailMessage();
            message.From = new MailAddress("s0534145423@gmail.com");
            message.To.Add(toEmail);
            message.Subject = "זכית בהגרלה!";
            message.Body = $"שלום {user.FirstName} {user.LastName},\n\nזכית במתנה: {gift.Name}\n\nמזל טוב!";

            try
            {
                using var smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("s0534145423@gmail.com", "Sari5423");
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
                logger.LogInformation("SendingEmail: email sent successfully to {Email}.", toEmail);
                return true;
            }
            catch (SmtpException smtpEx)
            {
                logger.LogError(smtpEx, "SendingEmail: SMTP error while sending email to {Email}.", toEmail);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SendingEmail: unexpected error while sending email to {Email}.", toEmail);
                throw;
            }
        }

        private static DtoLottery MapToResponseDto(Lottery lottery)
        {
            return new DtoLottery
            {
                GiftName = lottery.Gift.Name,
                UserName = lottery.User.FirstName + " " + lottery.User.LastName
            };
        }
    }
}
