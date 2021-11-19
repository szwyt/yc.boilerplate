using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YC.ElasticSearch;
using YC.ElasticSearch.Models;
using YC.FreeSqlFrameWork;
using YC.Micro.BookWebService;

namespace YC.Micro.BookWebService
{
    [Authorize]
    public class BookService : IBookService.IBookServiceBase
    {
        private IElasticSearchRepository<Book> _elasticSearchRepository;
        private readonly IMapper _mapper;

        public BookService(IElasticSearchRepository<Book> elasticSearchRepository, IMapper mapper)
        {
            _elasticSearchRepository = elasticSearchRepository;
            _mapper = mapper;
        }

        public override async Task<BookDtoList> GetBookList(BookFormRequest request, ServerCallContext context)
        {
            Func<QueryContainerDescriptor<Book>, QueryContainer> query = null;
            //ȫ��ƥ��+ �ִʲ�ѯ double ����ֱ����string ����ȥ��ѯ
            query = q => q.Term(t => t.BookName, request.QueryFilterString) ||
                  //q.Term(t => t.Price, "23") ||
                  q.Match(mq => mq.Field(f => f.BookContent).Query(request.QueryFilterString).Operator(Operator.And)) ||
                   q.Match(mq => mq.Field(f => f.Auther).Query(request.QueryFilterString).Operator(Operator.And));

            //������������ʾ
            Func<HighlightDescriptor<Book>, IHighlight> highlight = h => h.PreTags("<b class='key' style='color:red'>")
              .PostTags("</b>").Fields(f => f.Field(ff => ff.BookName), f => f.Field(ff => ff.BookContent),
              f => f.Field(ff => ff.Auther));
            var esPageResult = await _elasticSearchRepository.GetPageByQueryAsync(query, request.CurrentPage, request.PageSize, null, highlight);
            List<Book> list = esPageResult.List.ToList();
            long total = esPageResult.Total >= 10000 ? 10000 : esPageResult.Total;//��ѯ����,�������10000 Ĭ����ʾ10000������es��ȷ�ҳ��Ҫ������ʹ��searchAfter

            #region �������ݴ���

            if (list.Count > 0)
            {
                list.ForEach(
                    x =>
                    {
                        var tempHighlight = esPageResult.Hits.Where(t => t.Id.Contains(x.Id.ToString())).FirstOrDefault().Highlight;
                        IReadOnlyCollection<string> bookNameHighlightList;
                        tempHighlight.TryGetValue("bookName", out bookNameHighlightList);
                        if (bookNameHighlightList?.Count > 0)
                        {
                            x.BookName = "";//��ȡֵ��Ϊ�գ���ôԭ�е��������µ����
                            bookNameHighlightList.ToList().ForEach(v =>
                            {
                                x.BookName += v;
                            });
                        }

                        IReadOnlyCollection<string> bookContentHighlightList;
                        tempHighlight.TryGetValue("bookContent", out bookContentHighlightList);
                        if (bookContentHighlightList?.Count > 0)
                        {
                            x.BookContent = "";//��ȡֵ��Ϊ�գ���ôԭ�е��������µ����
                            bookContentHighlightList.ToList().ForEach(v =>
                            {
                                x.BookContent += v;
                            });
                        }

                        IReadOnlyCollection<string> autherHighlightList;
                        tempHighlight.TryGetValue("auther", out autherHighlightList);
                        if (autherHighlightList?.Count > 0)
                        {
                            x.Auther = "";//��ȡֵ��Ϊ�գ���ôԭ�е��������µ����
                            autherHighlightList.ToList().ForEach(v =>
                            {
                                x.Auther += v;
                            });
                        }
                    }
                    );
            }

            #endregion �������ݴ���

            var result = _mapper.Map<BookDtoList>(list);

            return result;
        }
    }
}