using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegraph.Net;
using Telegraph.Net.Models;
using VkNet.Model;
using VkNet.Model.Attachments;
using VKPostman.Models;

namespace VKPostman.Services
{
    public class TelegraphService
    {
        static TelegraphClient client;
        static ITokenClient tokenClient;
        static TelegraphService()
        {
            client = new TelegraphClient();            
            tokenClient = client.GetTokenClient(AppSettings.TelegraphApiKey);
        }
    
        internal static async Task<string> GetTelegraphPageAsync(Group page, Post post)
        {
            List<NodeElement> nodes = new List<NodeElement>();           
            NodeElement[] layout = PrepareLayout(nodes, page, post);

            Telegraph.Net.Models.Page newPage = await tokenClient.CreatePageAsync(
            page.Name,
            layout, 
            returnContent: true
            );
            return newPage.Url;
        }

        private static NodeElement[] PrepareLayout(List<NodeElement> nodes, Group page, Post post)
        {
            nodes.Add(PrepareInfo(nodes, page, post));
            nodes.Add(PrepareText(nodes,post));
            nodes.Add(PrepareAudio(nodes, post.Attachments));
            nodes.AddRange(PrepareImages(nodes, post.Attachments));
            return nodes.ToArray<NodeElement>();
        }
        private static NodeElement PrepareInfo(List<NodeElement> nodes, Group page, Post post)
        {
            return String.Format("#{0} | Ссылка на пост: https://vk.com/wall-{1}_{2}\n\n", page.ScreenName, page.Id, post.Id);
        }
        private static NodeElement PrepareText(List<NodeElement> nodes, Post post)
        {
            return post.Text.Length != 0 ? post.Text + "\n\n" : null;
        }
        private static NodeElement PrepareAudio(List<NodeElement> nodes, ReadOnlyCollection<Attachment> attachments)
        {
            List<Audio> audios = new List<Audio>();  
            foreach (var attachment in attachments)
            {
                if (attachment.Type == typeof(Audio))
                {
                    audios.Add(attachment.Instance as Audio);
                }

            }

            StringBuilder nodeBuilder = new StringBuilder();
            foreach (var audio in audios)
            {
                nodeBuilder.Append(String.Format("🎧 {0} - {1}\n", audio.Artist, audio.Title));
                
            }
            if (audios.Count>0) nodeBuilder.Append("\n");
            return nodeBuilder.ToString();
        }
        private static NodeElement[] PrepareImages(List<NodeElement> nodes, ReadOnlyCollection<Attachment> attachments)
        {
            List<NodeElement> range = new List<NodeElement>();
            List<Photo> photos = new List<Photo>();
            foreach (var attachment in attachments)
            {
                if (attachment.Type == typeof(Photo))
                {
                    photos.Add(attachment.Instance as Photo);
                }              
            }        
            foreach (var photo in photos)
            {
                string url = photo.Photo604.AbsoluteUri;
                int flag = 1;
                var node = new NodeElement
                {
                    Tag = "img",
                    Attributes = new Dictionary<string, string> { { "src", url } }
                };
                range.Add(node);

            }
            //TODO: Add video support
            return range.ToArray<NodeElement>();
        }
    }
}
