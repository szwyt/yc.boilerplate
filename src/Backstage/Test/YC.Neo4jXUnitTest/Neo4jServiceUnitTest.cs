using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using YC.Neo4j;
using YC.Neo4jXUnitTest.TestModel;

namespace YC.Neo4jXUnitTest
{
    public class Neo4jServiceUnitTest
    {
        public Neo4jRepository neo4jRepository;
        public List<UserInfo> userList;
        public List<Company> companyList;
        public Neo4jServiceUnitTest()
        {
            neo4jRepository = new Neo4jRepository("testdb");
            userList = new List<UserInfo>();
            userList.Add(new UserInfo() { Key = Guid.NewGuid().ToString(), Name = "����", Sex = "��", Type = "��ͨ�û�" });
            userList.Add(new UserInfo() { Key = Guid.NewGuid().ToString(), Name = "��˹", Sex = "��", Type = "�߼��û�" });
            userList.Add(new UserInfo() { Key = Guid.NewGuid().ToString(), Name = "����", Sex = "��", Type = "��ͨ�û�" });
            userList.Add(new UserInfo() { Key = Guid.NewGuid().ToString(), Name = "��С��", Sex = "Ů", Type = "�߼��û�" });
            companyList = new List<Company>();
            companyList.Add(new Company() { Key = Guid.NewGuid().ToString(), CompanyName = "��ȿƼ�", CEO = "����", Supervisor = "��С��", Type = "�Ƽ�" });
            companyList.Add(new Company() { Key = Guid.NewGuid().ToString(), CompanyName = "ǩ�ȿƼ�", CEO = "����", Supervisor = "��С��", Type = "�Ƽ�" });
            companyList.Add(new Company() { Key = Guid.NewGuid().ToString(), CompanyName = "��������", CEO = "��С��", Supervisor = "��СС", Type = "����" });
            companyList.Add(new Company() { Key = Guid.NewGuid().ToString(), CompanyName = "�ܷ�ʵҵ", CEO = "��˹", Supervisor = "��СС", Type = "ʵҵ" });


        }

        /// <summary>
        /// ����һ����¼
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateTest()
        {
            UserInfo u1 = new UserInfo();
            u1.Key = Guid.NewGuid().ToString();
            u1.Name = "��С";
            u1.Sex = "Ů";
            u1.Type = "�߼��û�";

            var result = await neo4jRepository.CreateSingleNode<UserInfo>("UserInfo", u1);

            Assert.Equal(1, result.Counters.NodesCreated);
        }

        /// <summary>
        /// ����Ĭ�ϲ�������
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateDataTest()
        {
            bool state = true;
            try
            {
                foreach (var u in userList)
                {
                    await neo4jRepository.CreateSingleNode<UserInfo>("UserInfo", u);
                }
                foreach (var c in companyList)
                {
                    await neo4jRepository.CreateSingleNode<Company>("Company", c);
                }
            }
            catch (Exception ex)
            {

                state = false;
            }
            Assert.True(state);
        }

        /// <summary>
        /// ���� �ƿ� �������ݹ���
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task MatchRelationControlTest()
        {
            string condition = $"{neo4jRepository.LeftKey}.Name={neo4jRepository.RightKey}.CEO";
            var result = await neo4jRepository.MatchNodeByProperty("UserInfo", "Company", "��Ȩ", $"{neo4jRepository.LeftKey}.Name", condition);
            Assert.True(result.Counters.ContainsUpdates);
        }

        /// <summary>
        /// ���� ��˾���� �������ݹ���
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task MatchRelationLinkTest()
        {
            string condition = $"{neo4jRepository.LeftKey}.Name={neo4jRepository.RightKey}.Supervisor";
            var result = await neo4jRepository.MatchNodeByProperty("UserInfo", "Company", "��˾����", $"{neo4jRepository.LeftKey}.Name", condition);
            Assert.True(result.Counters.ContainsUpdates);
        }

        /// <summary>
        /// ���½ڵ���Ϣ
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpateNodeTest()
        {
            string condtion = "Name:'��˹'";
            string setStr = $"{neo4jRepository.Key}.Name='��˹��',{neo4jRepository.Key}.Type='VIP�û�'";
            var result = await neo4jRepository.UpdateNode("UserInfo", condtion, setStr);

            Assert.True(result.Counters.ContainsUpdates);
            Assert.Equal(2, result.Counters.PropertiesSet);
        }

        /// <summary>
        /// ��ѯָ���ڵ���Ϣ
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SelectNodeTest()
        {
            string condtion = $"{neo4jRepository.Key}.Name='��С��'";
            var list = await neo4jRepository.SelectNode<UserInfo>("UserInfo", condtion);
            Assert.True(list.Count > 0);

        }

        /// <summary>
        /// ͨ�����ӹ�ϵ ��ѯָ���ڵ���Ϣ
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SelectNodeByRelationShipTest()
        {
            string relationShipName = "��˾����";
            string condition = "UserInfo.Name='��С��'";
            var tupleList = await neo4jRepository.SelectNodeByRelationShoip<UserInfo,Company>("UserInfo", "Company",relationShipName, condition);
            Assert.True(tupleList.Item1.Count>0);
            Assert.True(tupleList.Item2.Count>0);

        }

        [Fact]
        public async Task DeleteNodeTest()
        {
            string condtion = $"{neo4jRepository.Key}.Sex='��'";
            var result = await neo4jRepository.DeleteNode("UserInfo", condtion, true);
            Assert.True(result.Counters.NodesDeleted == 1);
        }
    }
}
