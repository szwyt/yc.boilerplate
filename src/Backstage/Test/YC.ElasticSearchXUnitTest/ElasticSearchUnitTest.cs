using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using YC.Core;
using YC.ElasticSearch;
using YC.Model.DbEntity;

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

                Id = ObjectId.Get(),
                BookName = "��������",
                Auther = "J.K.����",
                BookContent = " ���嵽��·���棬�ص��칫�ң������Ը����鲻Ҫ��������Ȼ��ץ��Ͳ����Ҫ��ͨ����ĵ绰����ʱ�ֱ����ԡ������»�Ͳ�����ź��룬��ĥ��������������̫�޴��ˡ����ز�����һ��ϡ�е��գ��϶���������ղ��أ������ж��ӽй������뵽������������Լ��������ǲ��ǽй������ò����ˡ�������û�����⺢�ӡ�˵�����й�ά�����߽й��޵¡�û�б�Ҫ��̫̫���ģ�ֻҪһ���������ã��������ķ����ҡ����������������Ҫ�����Լ���һ�������������ء����ɲ�����ô˵����Ⱥ��������ˡ���",
                PublishDate = DateTime.Parse("1997��6��30��"),
                Price = 57.9
            };
            var result = await _elasticSearchRepository.CreateAsync(book);
            Assert.NotNull(result);
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

                Id = ObjectId.Get(),
                BookName = "����֮��",
                Auther = "��ӥ׽С��",
                BookContent = "�����������ʵ���������Ǿ�ƭ���ˣ�ƭ����ļһ����û��˿������У�����ǵ�����������",
                PublishDate = DateTime.Parse("2020��6��30��"),
                Price = 67.3
            });
            bookList.Add(new Book()
            {

                Id = ObjectId.Get(),
                BookName = "����",
                Auther = "��ӥ׽С��",
                BookContent = "��������������ȥ������ɣ�Ҳ���Ǳ߻���һЩ�ջ��Ǳ��и�����սʿ�����ǱȽ�ţ���ϴ�������������ģ���β�֪���᲻��̬�Ⱥ�һ�㡣��",
                PublishDate = DateTime.Parse("2021��7��21��"),
                Price = 36
            });
            var result = await _elasticSearchRepository.CreateListAsync(bookList);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetTest()
        {
            var result = await _elasticSearchRepository.MultiGetAsync();
            Assert.NotNull(result);
        }

    }
}
