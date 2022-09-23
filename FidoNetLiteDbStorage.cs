#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib.Objects;
using LiteDB;

namespace Fido2NetLib.Development
{
    public class LiteDbStorage
    {
        //private readonly ConcurrentDictionary<string, Fido2User> _storedUsers = new();
        //private readonly List<MStoredCredential> _storedCredentials = new();

        public static ILiteDatabase Db = new LiteDatabase(@"MyData.db");
        public static ILiteCollection<Fido2User> ColUser = Db.GetCollection<Fido2User>();
        public static ILiteCollection<MStoredCredential> ColCert = Db.GetCollection<MStoredCredential>();

        public Fido2User GetOrAddUser(string username, Func<Fido2User> addCallback)
        {
            var i = ColUser.Find(x => x.Name == (username)).ToList();
            if (i.Any()) return i.FirstOrDefault()!;

            var j = addCallback.Invoke();
            ColUser.Insert(j);
            return j; }

        public Fido2User? GetUser(string username)
        {
            return ColUser.Find(x => x.Name == (username)).ToList().FirstOrDefault();
        }

        public List<MStoredCredential> GetCredentialsByUser(Fido2User user)
        {
            return ColCert.Find(c => c.UserId == (user.Id)).ToList();
        }

        public MStoredCredential? GetCredentialById(byte[] id)
        {
            return ColCert.Find(c => c.Descriptor.Id == (id)).ToList().FirstOrDefault();
        }

        public Task<List<MStoredCredential>> GetCredentialsByUserHandleAsync(byte[] userHandle,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ColCert.Find(c => c.UserHandle == (userHandle)).ToList());
        }

        public void UpdateCounter(byte[] credentialId, uint counter)
        {
            var cred = ColCert.Find(c => c.Descriptor.Id == (credentialId)).First();
            cred.SignatureCounter = counter;
        }

        public void AddCredentialToUser(Fido2User user, MStoredCredential credential)
        {
            credential.UserId = user.Id;
            ColCert.Insert(credential);
        }

        public Task<List<Fido2User>> GetUsersByCredentialIdAsync(byte[] credentialId,
            CancellationToken cancellationToken = default)
        {
            // our in-mem storage does not allow storing multiple users for a given credentialId. Yours shouldn't either.
            var cred = ColCert.Find(c => c.Descriptor.Id == (credentialId)).FirstOrDefault();

            if (cred is null)
                return Task.FromResult(new List<Fido2User>());

            return Task.FromResult(ColUser.Find(x => x.Id == (cred.UserId)).ToList());
        }
    }

#nullable disable

    public class MStoredCredential
    {
        public byte[] UserId { get; set; }
        public PublicKeyCredentialDescriptor Descriptor { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] UserHandle { get; set; }
        public uint SignatureCounter { get; set; }
        public string CredType { get; set; }
        public DateTime RegDate { get; set; }
        public Guid AaGuid { get; set; }
    }
}
