using System;
using System.Threading.Tasks;
using Xunit;
using YC.Neo4j;
using YC.Neo4jXUnitTest.TestModel;

namespace YC.Neo4jXUnitTest
{
    public class Neo4jServiceUnitTest
    {
        Neo4jService neo4jService;

        public Neo4jServiceUnitTest()
        {
            neo4jService = new Neo4jService("testdb");
        }
        [Fact]
        public async Task CreateTest()
        {
            UserInfo u1 = new UserInfo();
            u1.Key = Guid.NewGuid().ToString();
            u1.Name = "��С�";
            u1.Sex = "Ů";
            u1.Remark = "�߼��û�";
            var result = await neo4jService.CreateSingleNode<UserInfo>("UserInfo", u1);
            Assert.Equal(1,result.Counters.NodesCreated);
        }
        [Fact]
        public async Task MatchRelationTest()
        {
            string condition = "a.Remark=b.Remark and a.Key<>b.Key";
            var result = await neo4jService.MatchNodeByProperty("UserInfo", "UserInfo", "sameRemark", "a.Remark", condition);
            Assert.True(result.Counters.ContainsUpdates);
        }

        [Fact]
        public async Task UpateNodeTest()
        {
            string condtion = "Name:'��С��',Sex:'Ů'";
            string setStr = "n.Name='��С��',n.Remark='��ͨ�û�'";
            var result = await neo4jService.UpdateNode("UserInfo", condtion, setStr);

            Assert.True(result.Counters.ContainsUpdates);
            Assert.Equal(2, result.Counters.PropertiesSet);
        }

        [Fact]
        public async Task SelectNodeTest()
        {
            string condtion = "n.Sex='Ů'";
          var list= await neo4jService.SelectNode<UserInfo>("UserInfo", condtion);

            Assert.True(list.Count > 0);
           
        }
    }
}
