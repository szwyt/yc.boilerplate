using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using YC.Common.ShareUtils;
using YC.Core;
using YC.ElasticSearch;
using YC.Model.DbEntity;
using System.Linq;
using Nest;

namespace YC.ElasticSearchXUnitTest
{
    public class ElasticSearchUnitTest
    {
        public IElasticSearchDbContext _elasticSearchDbContext;
        public IElasticSearchRepository<Book> _elasticSearchRepository;
        public ITenant _tenant;

        public ElasticSearchUnitTest()
        {

            string[] strArray = new string[] { "http://118.25.208.8:9200" };
            _elasticSearchDbContext = new ElasticSearchDbContext(strArray);
            _tenant = new TestTenant();
            _tenant.TenantId = 1;
            //_tenant.TenantDbString = "";//�ɿգ�ʵ��ҵ���߼�û���õ�
            _elasticSearchRepository = new ElasticSearchRepository<Book>(_elasticSearchDbContext, _tenant);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateAsyncTest()
        {
            var book = new Book()
            {

                Id = "61555adc91ef624a58fda092",
                BookName = "��������",
                Auther = "J.K.����",
                BookContent = " ���嵽��·���棬�ص��칫�ң������Ը����鲻Ҫ��������Ȼ��ץ��Ͳ����Ҫ��ͨ����ĵ绰����ʱ�ֱ����ԡ������»�Ͳ�����ź��룬��ĥ��������������̫�޴��ˡ����ز�����һ��ϡ�е��գ��϶���������ղ��أ������ж��ӽй������뵽������������Լ��������ǲ��ǽй������ò����ˡ�������û�����⺢�ӡ�˵�����й�ά�����߽й��޵¡�û�б�Ҫ��̫̫���ģ�ֻҪһ���������ã��������ķ����ҡ����������������Ҫ�����Լ���һ�������������ء����ɲ�����ô˵����Ⱥ��������ˡ���",
                PublishDate = DateTime.Parse("1997��6��30��"),
                Price = 57.9
            };
            string jsonData = book.ToJson();
            var result = await _elasticSearchRepository.CreateAsync(book);

            Assert.True(result.State);

        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreatListAsyncTest()
        {
            List<Book> bookList = new List<Book>();
            bookList.Add(new Book()
            {
                //Id= "615530ec3d356c46d4463e74",
                Id = ObjectId.Get(),
                BookName = "����123",
                Auther = "��ӥ׽С��12121",
                BookContent = "�����������ʵ���������Ǿ�ƭ���ˣ�ƭ����ļһ����û��˿������У�����ǵ�����������",
                PublishDate = DateTime.Parse("2020��6��30��"),
                Price = 67.3
            });
            bookList.Add(new Book()
            {
                //Id = "615530ec3d356c46d4463e75",
                Id = ObjectId.Get(),
                BookName = "����121",
                Auther = "��ӥ׽С��",
                BookContent = "��������������ȥ������ɣ�Ҳ���Ǳ߻���һЩ�ջ��Ǳ��и�����սʿ�����ǱȽ�ţ���ϴ�������������ģ���β�֪���᲻��̬�Ⱥ�һ�㡣��",
                PublishDate = DateTime.Parse("2021��7��21��"),
                Price = 36
            }); ; ;
            string jsonData = bookList.ToIndentedJson();
            var result = await _elasticSearchRepository.CreateListAsync(bookList);
            Assert.True(result.ApiCall.Success);//api �����Ƿ�����
            //ҵ������Ƿ�ɹ�
            var resultCount = result.Items.Where(x => x.Result == Nest.Result.Created.ToString().ToLower()).Count();
            Assert.True(resultCount == 2);
        }


        /// <summary>
        /// ��������,��У���Ƿ����
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreatListByExistValidateAsyncTest()
        {
            List<Book> bookList = new List<Book>();
            bookList.Add(new Book()
            {
                //Id= "615530ec3d356c46d4463e74",
                Id = ObjectId.Get(),
                BookName = "����֮��099",
                Auther = "��ӥ׽С��",
                BookContent = "�����������ʵ���������Ǿ�ƭ���ˣ�ƭ����ļһ����û��˿������У�����ǵ�����������",
                PublishDate = DateTime.Parse("2020��6��30��"),
                Price = 67.3
            });
            bookList.Add(new Book()
            {
                //Id = "615530ec3d356c46d4463e75",
                Id = ObjectId.Get(),
                BookName = "����099",
                Auther = "��ӥ׽С��",
                BookContent = "��������������ȥ������ɣ�Ҳ���Ǳ߻���һЩ�ջ��Ǳ��и�����սʿ�����ǱȽ�ţ���ϴ�������������ģ���β�֪���᲻��̬�Ⱥ�һ�㡣��",
                PublishDate = DateTime.Parse("2021��7��21��"),
                Price = 36
            });
            string jsonData = bookList.ToIndentedJson();

            var result = await _elasticSearchRepository.CreateListByExistValidateAsync(bookList);

            Assert.True(result.Data == 2);
        }

        /// <summary>
        /// ͨ��_id ��ȡ������¼
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAsyncTest()
        {
            string id = "61555adc91ef624a58fda092";
            var result = await _elasticSearchRepository.GetAsync(id);
            Assert.NotNull(result);
        }

        /// <summary>
        /// ɾ��ָ��id����
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteByIdTest()
        {
            string id = "61556dc18820402f3863c930";
            var result = await _elasticSearchRepository.DeleteByIdAsync(id);
            Assert.True(result.Result == Nest.Result.Deleted);
        }

        /// <summary>
        /// ɾ����������
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteAllTest()
        {
            var result = await _elasticSearchRepository.DeleteAllAsync();
            Assert.True(result.Deleted > 0);
        }

        /// <summary>
        /// ����ɾ��
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteByQueryTest()
        {
            Func<DeleteByQueryDescriptor<Book>, IDeleteByQueryRequest> deleteOp = d =>d.Index(_elasticSearchRepository.MappingName).Query(q =>q.Range(r => r.Field(f => f.Price).GreaterThanOrEquals(200))) ;
            //��ѯ����
            Func<QueryContainerDescriptor<Book>, QueryContainer> query = d =>
           d.Range(r => r.Field(f => f.Price).GreaterThanOrEquals(200));
            //��ѯ���
            var queryList = await _elasticSearchRepository.GetByQueryAsync(query);
            //ɾ�����
            var result=  await _elasticSearchRepository.DeleteByQueryAsync(deleteOp);

            Assert.NotNull(result);
        }

        /// <summary>
        /// ������ѯ
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetByQueryTest()
        {
            Func<QueryContainerDescriptor<Book>, QueryContainer> query = q => q.Match(mq =>
              mq.Field(f => f.BookName).Query("����123").Operator(Operator.And)
              )
              &&
               q.Range(mq =>
                mq.Field(f => f.Price).GreaterThanOrEquals(100)
              )
               ||
              q.Match(mq =>
              mq.Field(f => f.Auther).Query("��ӥ׽С��12121").Operator(Operator.And)
              )
            ;//���Operator.Or,��ô�����ִ�֮��������[����]��Ҳ�����

            //Func<QueryContainerDescriptor<Book>, QueryContainer> query1 = q => q.Range(mq =>
            //mq.Field(f=>f.Price).
            // );

            var result = await _elasticSearchRepository.GetByQueryAsync(query);
            List<Book> list = result.Documents.ToList();
            Assert.True(list.Count > 0);
        }


        /// <summary>
        /// ͨ��id ����
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdateTest()
        {
            string id = "6155a94eaa40676904f987b9";
            Func<QueryContainerDescriptor<Book>, QueryContainer> query = q => q.Match(mq =>
             mq.Field(f => f.Id).Query(id).Operator(Operator.And)
             );
            var queryList = await _elasticSearchRepository.GetByQueryAsync(query);
            var obj = queryList.Documents.FirstOrDefault();
            obj.Price = 200;
            var result = await _elasticSearchRepository.UpdateAsync(id, obj);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task AllTest()
        {

            var result = await _elasticSearchDbContext.Client.UpdateByQueryAsync<Book>(s => s
                              .Index(_elasticSearchRepository.MappingName) //or specify index via 
                             .Query(q => q.Match(mq => mq.Field(f => f.BookName).Query("����123").Operator(Operator.And))));

        }




    }
}
