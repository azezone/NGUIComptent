using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class TexLRUCache
{
    private int _size;//链表长度
    private int _capacity;//缓存容量 
    private Dictionary<string, ListNode<TextureData>> _dic;//key +缓存数据
    private ListNode<TextureData> _linkHead;
    public TexLRUCache(int capacity)
    {
        _linkHead = new ListNode<TextureData>("", default(TextureData));
        _linkHead.Next = _linkHead.Prev = _linkHead;
        this._size = 0;
        this._capacity = capacity;
        this._dic = new Dictionary<string, ListNode<TextureData>>();
    }

    public TextureData Get(string key)
    {
        if (!string.IsNullOrEmpty(key) && _dic.ContainsKey(key))
        {
            ListNode<TextureData> n = _dic[key];
            MoveToHead(n);
            n.Value.usedTimes++;
            return n.Value;
        }
        else
        {
            return null;
        }
    }

    public void Set(string key, TextureData value)
    {
        if (string.IsNullOrEmpty(key) || value == null || value.tex == null)
        {
            return;
        }

        ListNode<TextureData> n;
        if (_dic.ContainsKey(key))
        {
            n = _dic[key];
            n.Value = value;
            n.Value.usedTimes++;
            MoveToHead(n);
        }
        else
        {
            n = new ListNode<TextureData>(key, value);
            AttachToHead(n);
            n.Value.usedTimes++;
            _size++;
            _dic.Add(key, n);
        }

        if (_size > _capacity)
        {
            RemoveUnusedTex();      //超出容量之后remove掉无引用texture
        }
    }

    public void Unload(string key)
    {
        if (!string.IsNullOrEmpty(key) && _dic.ContainsKey(key))
        {
            ListNode<TextureData> n = _dic[key];
            n.Value.usedTimes--;
        }
    }

    private int _accDeCount = 0;    //累计清除次数
    // 从链表尾部依次移除无应用texture
    private void RemoveUnusedTex()
    {
        Debuger.LogError("RemoveUnusedTex!!!!!!");
        //Debuger.LogError(ToString());
        ListNode<TextureData> node = _linkHead.Prev;
        ListNode<TextureData> denode;
        int decount = 0;
        while (node != _linkHead && decount < _capacity / 2)
        {
            denode = node;
            node = node.Prev;
            if (denode.Value.usedTimes < 1)
            {
                Debuger.Log(NodeToString(denode));
                // destroy该tex
                GameObject.Destroy(denode.Value.tex);
                denode.Value.tex = null;

                // 从字典和list中remove该key
                _dic.Remove(denode.Key);
                RemoveFromList(denode);

                decount++;
                _size--;
            }
        }
        _accDeCount += decount;

        Debuger.LogError("RemoveUnusedTex decount is:" + decount + " accDeCount:" + _accDeCount);

        // 当_accDeCount到达一定数量才进行一次资源释放，防止频繁unload
        if (_accDeCount > _capacity / 2)
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            _accDeCount = 0;
        }

        Debuger.LogError("After RemoveUnusedTex");
    }

    // 移出链表最后一个节点
    private void RemoveLast()
    {
        ListNode<TextureData> deNode = _linkHead.Prev;
        RemoveFromList(deNode);
        _dic.Remove(deNode.Key);
    }

    // 将一个孤立节点放到头部
    private void AttachToHead(ListNode<TextureData> n)
    {
        n.Prev = _linkHead;
        n.Next = _linkHead.Next;
        _linkHead.Next.Prev = n;
        _linkHead.Next = n;
    }

    // 将一个链表中的节点放到头部
    private void MoveToHead(ListNode<TextureData> n)
    {
        RemoveFromList(n);
        AttachToHead(n);
    }

    private void RemoveFromList(ListNode<TextureData> n)
    {
        //将该节点从链表删除
        n.Prev.Next = n.Next;
        n.Next.Prev = n.Prev;
    }

    public override string ToString()
    {
        ListNode<TextureData> node = _linkHead.Next;
        StringBuilder sb = new StringBuilder();
        while (node != _linkHead)
        {
            sb.Append(node.Key);
            sb.Append(" ");
            node = node.Next;
        }
        return sb.ToString();
    }

    private string NodeToString(ListNode<TextureData> node)
    {
        return node.Key;
    }
}

public class ListNode<T>
{
    public ListNode<T> Prev;
    public ListNode<T> Next;
    public T Value;
    public string Key;

    public ListNode(string key, T val)
    {
        Value = val;
        Key = key;
        this.Prev = null;
        this.Next = null;
    }
}