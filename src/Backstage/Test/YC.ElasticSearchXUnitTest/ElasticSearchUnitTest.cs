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
using YC.ElasticSearch.Models;

namespace YC.ElasticSearchXUnitTest
{
    public class ElasticSearchUnitTest
    {
        public IElasticSearchDbContext _elasticSearchDbContext;
        public IElasticSearchRepository<Book> _elasticSearchRepository;
        public ITenant _tenant;

        public ElasticSearchUnitTest()
        {

            string[] strArray = new string[] { "http://127.0.0.1:9200" };
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
            var result = await _elasticSearchRepository.CreateListAsync(bookList,true);
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
                BookName = "��Ƥ��",
                Auther = "δ֪",
                BookContent = "����һ����",
                PublishDate = DateTime.Parse("2020��6��30��"),
                Price = 67.3
            });
            bookList.Add(new Book()
            {
                //Id = "615530ec3d356c46d4463e75",
                Id = ObjectId.Get(),
                BookName = "��������",
                Auther = "δ֪",
                BookContent = "��Ƥ",
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
        /// ������ѯ������������+������Χ��ѯ
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetByQueryTest()
        {
            Func<QueryContainerDescriptor<Book>, QueryContainer> query = q => q.Match(mq =>
              mq.Field(f => f.BookName).Query("����123").Operator(Operator.And)
              )
              &&
               q.TermRange(mq =>
                mq.Field(f => f.Price).GreaterThanOrEquals("19").LessThan("100")
              )//��Χ��ѯ
               ||
              q.Match(mq =>
              mq.Field(f => f.Auther).Query("��ӥ׽С��12121").Operator(Operator.And)
              )
            ;//���Operator.Or,��ô�����ִ�֮��������[����]��Ҳ�����

            //Func<QueryContainerDescriptor<Book>, QueryContainer> query1 = q => q.Range(mq =>
            //mq.Field(f=>f.Price).
            // );
            //����
            Func<SortDescriptor<Book>, IPromise<IList<ISort>>> sort = s => s.Ascending(a => a.PublishDate);
            var result = await _elasticSearchRepository.GetByQueryAsync(query, sort);
            List<Book> list = result.ToList();
            Assert.True(list.Count > 0);
        }



        /// <summary>
        /// ������ѯ,ȫ��ƥ��
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetByQueryKeyWordTest()
        {
             
            // "bookName" : {
            //"type" : "keyword"
            //},
            //BookName �޸�Ϊkeyword ���б�������ƥ�䣬���ִ�
            Func<QueryContainerDescriptor<Book>, QueryContainer> query1 = q => q.Term(t => t.BookName, "����123");
            Func<QueryContainerDescriptor<Book>, QueryContainer> query2 = q => q.Match(mq =>
              mq.Field(f => f.BookName).Query("����").Operator(Operator.Or)
              );//��������Ϊ keyword������Match ���Ҳ�������ֻ��ʹ��Term ��ȷ��ѯ
            var result = await _elasticSearchRepository.GetByQueryAsync(query1);
            List<Book> list = result.ToList();
            Assert.True(list.Count > 0);
        }

        /// <summary>
        /// ��ѯ��ȡ����
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllTest()
        {
            //����
            Func<SortDescriptor<Book>, IPromise<IList<ISort>>> sort = s => s.Ascending(a => a.PublishDate);
            var result = await _elasticSearchRepository.GetAllAsync(sort);
            List<Book> list = result.ToList();
            Assert.True(list.Count > 0);
        }

        /// <summary>
        /// ������ѯ,һ���ؼ��ʣ�����ֶ�ƥ��
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetByQueryMutiTest()
        {
            string searchKey = "��Ƥ";
           //�����ǵ���ƽ����ѯһ���ؼ��ʣ�����ֶ�ȥƥ�䣬ֻҪƥ�䵽����ʾ
            Func<QueryContainerDescriptor<Book>, QueryContainer> query = q => q.MultiMatch(m =>m.Fields(f=>f.Fields(ff=>ff.BookName).Fields(ff => ff.BookContent))
                        .Query(searchKey));
            var result = await _elasticSearchRepository.GetByQueryAsync(query);
            List<Book> list = result.ToList();
            Assert.True(list.Count > 0);
        }

        /// <summary>
        /// �ۺϲ�ѯ
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetByQueryAggregationsTest()
        {
            string searchKey = "��Ƥ";
            string avgKeyName = "BookPrice_Average";
            //1.ȫ��ƥ��
            Func<QueryContainerDescriptor<Book>, QueryContainer> query1 = q => q
                           .Term(t => t.BookName, searchKey);

            //2.����ѯ
            Func<QueryContainerDescriptor<Book>, QueryContainer> query2 = q => q
                      .Match(m=>m.Field(f=>f.BookName).Query(searchKey));
            Func<AggregationContainerDescriptor<Book>, IAggregationContainer> aggs1=a=>
               a.Average(avgKeyName, aa => aa.Field(f => f.Price));//ͳ��ƽ��
            Func<AggregationContainerDescriptor<Book>, IAggregationContainer> aggs2 = a =>
                a.ExtendedStats(avgKeyName, aa => aa.Field(f => f.Price));//ͳ������
            Func<AggregationContainerDescriptor<Book>, IAggregationContainer> aggs3 = a =>
              a.Max(avgKeyName, aa => aa.Field(f => f.Price));//���
            Func<AggregationContainerDescriptor<Book>, IAggregationContainer> aggs4 = a =>
              a.Percentiles(avgKeyName, aa => aa.Field(f => f.Price));//�ٷ���

           

            var result = await _elasticSearchRepository.GetByQueryAggregationsAsync(query2, aggs3);
            List<Book> list = result.Item1.ToList();
            var aggResult = result.Item2[avgKeyName];
            //AggregateDictionary;
            var data=(ExtendedStatsAggregate)aggResult;//����Ҫ���ݷ��صĽ��ת��ָ�������ͣ�ͨ��AggregateDictionary���Ҷ�Ӧ������
            var valueAggregate =((ValueAggregate)aggResult).Value;//��ȡ�õ�ָ����ƽ����
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
            var obj = queryList.FirstOrDefault();
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
