using BMK.Models.DbEntities;
using BMK.Models.ViewModels;
using BMK.UnitOfWork.Main;

using RxWeb.Core;
using RxWeb.Core.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Domain.Domain
{
    public class BmkScheduleMeetingDomain : IBmkScheduleMeetingDomain
    {
        private IUserUow Uow { get; set; }
        private HashSet<string> ValidationMessage { get; set; }
        private IUserClaim UserClaim { get; set; }
        public BmkScheduleMeetingDomain(IUserUow uow, IUserClaim userClaim)
        {
            Uow = uow;
            ValidationMessage = new HashSet<string>();
            UserClaim = userClaim;
        }

        public async Task<IEnumerable<BmkMemberMeeting>> GetAllAsync()
        {
            return await Uow.Repository<BmkMemberMeeting>().FindByAsync(b => (bool)b.IsActive);
        }

        public async Task<BmkMemberMeeting> GetByAsync(int id)
        {
            return await Uow.Repository<BmkMemberMeeting>().FindByKeyAsync(id);
        }

        public async Task<BmkMemberMeeting> AddAsync(BmkMemberMeeting bmkmemberMeeting)
        {
            bmkmemberMeeting = SetDefaultValue(bmkmemberMeeting);
            await Uow.RegisterNewAsync(bmkmemberMeeting);
            await Uow.CommitAsync();
            return bmkmemberMeeting;
        }

        public async Task<HashSet<string>> AddValidation(BmkMemberMeeting bmkMemberMeeting)
        {
            return await CommanValidation(bmkMemberMeeting);
        }
        public async Task<HashSet<string>> UpdateValidation(BmkMemberMeeting bmkMemberMeeting)
        {
            return await CommanValidation(bmkMemberMeeting);
        }

        private async Task<HashSet<string>> CommanValidation(BmkMemberMeeting bmkMemberMeeting)
        {
            IEnumerable<BmkMemberMeeting> activeMembers = await GetAllAsync();
            BmkMemberMeeting memberExists = activeMembers.FirstOrDefault(b => (bool)b.IsActive && b.MemberName == bmkMemberMeeting.MemberName);
            BmkMemberMeeting memberMeetingUrlExists = activeMembers.FirstOrDefault(b => (bool)b.IsActive && b.MeetingUrl == bmkMemberMeeting.MeetingUrl);

            if (memberExists != null && memberExists.BmkMemberMeetingId != bmkMemberMeeting.BmkMemberMeetingId)
            {
                ValidationMessage.Add("Bmk member with this name already exists.");
            }
            if (memberMeetingUrlExists != null && memberMeetingUrlExists.BmkMemberMeetingId != bmkMemberMeeting.BmkMemberMeetingId)
            {
                ValidationMessage.Add("Another bmk member has the same meeting url.");
            }

            return ValidationMessage;
        }

        public async Task<BmkMemberMeeting> UpdateAsync(BmkMemberMeeting bmkmemberMeeting)
        {
            bmkmemberMeeting = SetDefaultValue(bmkmemberMeeting);
            await Uow.RegisterDirtyAsync(bmkmemberMeeting);
            await Uow.CommitAsync();
            return bmkmemberMeeting;
        }

        public async Task<BmkMemberMeeting> DeleteAsync(BmkMemberMeeting bmkmemberMeeting)
        {
            await Uow.RegisterDirtyAsync(bmkmemberMeeting);
            await Uow.CommitAsync();
            return bmkmemberMeeting;
        }

        private BmkMemberMeeting SetDefaultValue(BmkMemberMeeting bmkMemberMeeting)
        {
            if (bmkMemberMeeting != null)
            {
                if (bmkMemberMeeting.BmkMemberMeetingId == 0)
                {
                    bmkMemberMeeting.CreatedDate = DateTime.Now;
                    bmkMemberMeeting.CreatedBy = UserClaim.UserId;
                }
                else
                {
                    bmkMemberMeeting.ModifiedDate = DateTime.Now;
                    bmkMemberMeeting.ModifiedBy = UserClaim.UserId;
                }
            }
            return bmkMemberMeeting;
        }

    }
    public interface IBmkScheduleMeetingDomain
    {
        Task<IEnumerable<BmkMemberMeeting>> GetAllAsync();
        Task<BmkMemberMeeting> GetByAsync(int id);
        Task<BmkMemberMeeting> AddAsync(BmkMemberMeeting bmkmemberMeeting);
        Task<BmkMemberMeeting> UpdateAsync(BmkMemberMeeting bmkmemberMeeting);
        Task<BmkMemberMeeting> DeleteAsync(BmkMemberMeeting bmkmemberMeeting);
        Task<HashSet<string>> AddValidation(BmkMemberMeeting bmkMemberMeeting);
        Task<HashSet<string>> UpdateValidation(BmkMemberMeeting bmkMemberMeeting);
    }
}
