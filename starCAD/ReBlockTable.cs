using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace starCAD
{
    public static class ReBlockTable
    {
        public static string GetCurrentSpaceName()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // 获取当前的空间ID
                ObjectId spaceId = db.CurrentSpaceId;
                if (spaceId.IsNull)
                {
                    Editor ed = doc.Editor;
                    ed.WriteMessage("\nNo current space.");
                    return string.Empty;
                }

                // 获取空间名称
                BlockTableRecord space = tr.GetObject(spaceId, OpenMode.ForRead) as BlockTableRecord;
                return space.Name;
            }
        }

        public static void ReBlock(Entity en)
        {
            //声明图形数据库对象
            DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();
            //Database db = Application.DocumentManager.MdiActiveDocument.Database;
            Database db = HostApplicationServices.WorkingDatabase;

            //Document doc = Application.DocumentManager.MdiActiveDocument;
            //Database db = doc.Database;
            //开启事务处理
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //打开块表
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                //打开块表记录
                BlockTableRecord btr = trans.GetObject(bt[GetCurrentSpaceName()], OpenMode.ForWrite) as BlockTableRecord;
                //加直线到块表记录
                btr.AppendEntity(en);
                //更新数据
                trans.AddNewlyCreatedDBObject(en, true);
                //事务提交
                trans.Commit();
            }
            docLock.Dispose();
        }

        public static void ReBlock(IEnumerable<Entity> en)
        {
            //声明图形数据库对象
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            //Document doc = Application.DocumentManager.MdiActiveDocument;
            //Database db = doc.Database;
            //开启事务处理
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //打开块表
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                //打开块表记录
                BlockTableRecord btr = trans.GetObject(bt[GetCurrentSpaceName()], OpenMode.ForWrite) as BlockTableRecord;
                //加直线到块表记录
                foreach (var item in en)
                {
                    btr.AppendEntity(item);
                    //更新数据
                    trans.AddNewlyCreatedDBObject(item, true);
                    //事务提交
                }
                trans.Commit();
            }
        }

        public static void ReBlock(Entity[] en, Entity[] Ref)
        {
            //声明图形数据库对象
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            //Document doc = Application.DocumentManager.MdiActiveDocument;
            //Database db = doc.Database;
            //开启事务处理
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //打开块表
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                //打开块表记录
                BlockTableRecord btr = trans.GetObject(bt[GetCurrentSpaceName()], OpenMode.ForWrite) as BlockTableRecord;
                //加直线到块表记录
                for (int i = 0; i < en.Count(); i++)
                {
                    Entity entity = (Entity)trans.GetObject(Ref[i].Id, OpenMode.ForWrite);
                    en[i].Layer = entity.Layer;
                    btr.AppendEntity(en[i]);
                    //更新数据
                    trans.AddNewlyCreatedDBObject(en[i], true);
                    entity.Erase(true);
                    //事务提交
                }
                trans.Commit();
            }
        }

        public static void ReBlock(Database db, Entity en)
        {

            //Document doc = Application.DocumentManager.MdiActiveDocument;
            //Database db = doc.Database;
            //开启事务处理
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument();

                //打开块表
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                //打开块表记录
                BlockTableRecord btr = trans.GetObject(bt[GetCurrentSpaceName()], OpenMode.ForWrite) as BlockTableRecord;
                //加直线到块表记录
                btr.AppendEntity(en);
                //更新数据
                trans.AddNewlyCreatedDBObject(en, true);

                //事务提交
                trans.Commit();
                docLock.Dispose();
            }
        }

        public static void ReBlock(Database db, Entity[] en)
        {

            //Document doc = Application.DocumentManager.MdiActiveDocument;
            //Database db = doc.Database;
            //开启事务处理
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //打开块表
                BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                //打开块表记录
                BlockTableRecord btr = trans.GetObject(bt[GetCurrentSpaceName()], OpenMode.ForWrite) as BlockTableRecord;
                //加直线到块表记录
                for (int i = 0; i < en.Length; i++)
                {
                    btr.AppendEntity(en[i]);
                    //更新数据
                    trans.AddNewlyCreatedDBObject(en[i], true);
                    //事务提交
                }
                trans.Commit();
            }
        }
        public static PromptSelectionResult GetSeltctOp(this Editor ed, string promptStr)
        {
            // 声明一个获取点的提示类
            PromptSelectionResult ppo = ed.GetSelection();
            // 使回车和空格键有效
            return ed.GetSeltctOp(promptStr);
        }

        public static PromptKeywordOptions GetSelectOp2(this Editor ed, string promtpstr, params string[] keyword)
        {
            PromptKeywordOptions pso = new PromptKeywordOptions(promtpstr);
            for (int i = 0; i < keyword.Length; i++)
            {

                pso.Keywords.Add(keyword[i]);
            }
            pso.AppendKeywordsToMessage = false;
            //pso.AllowSpaces = true;
            //pso.AppendKeywordsToMessage = false;
            return pso;
        }

        public static PromptEntityOptions GetSelectEntityOp2(this Editor ed, string promtpstr, params string[] keyword)
        {
            PromptEntityOptions pso = new PromptEntityOptions(promtpstr);
            for (int i = 0; i < keyword.Length; i++)
            {

                pso.Keywords.Add(keyword[i]);
            }
            pso.AppendKeywordsToMessage = false;
            //pso.AllowSpaces = true;
            //pso.AppendKeywordsToMessage = false;
            return pso;
        }
    }

    public static class ReBlockTool
    {
        public static ObjectId AddBlock(this Database db, string btrName, List<Entity> ents, Point3d ori)
        {
            ObjectId objectId = ObjectId.Null;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                if (!bt.Has(btrName))
                {
                    BlockTableRecord btr = new BlockTableRecord();
                    btr.Name = btrName;
                    btr.ResetScaleDependentProperties();
                    for (int i = 0; i < ents.Count; i++)
                    {
                        btr.AppendEntity(ents[i]);
                    }
                    bt.UpgradeOpen();
                    bt.Add(btr);
                    trans.AddNewlyCreatedDBObject(btr, true);
                    bt.DowngradeOpen();
                    //btr.Origin = ori;
                }
                objectId = bt[btrName];
                trans.Commit();
            }
            return objectId;
        }
    }
}
