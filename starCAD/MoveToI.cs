using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;
using starCAD;
using aai = Autodesk.AutoCAD.Interop;
using aaic = Autodesk.AutoCAD.Interop.Common;

namespace sCAD
{
    public class MoveToI
    {
        [CommandMethod("OpenNHL")]

        public void OpenNHL()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            db.ObjectAppended += Db_ObjectAppended;
        }

        public List<ObjectId> objectIds = new List<ObjectId>();
        public List<DBObject> DBObjects = new List<DBObject>();
        public ObjectIdCollection occ = new ObjectIdCollection();
        private void Db_ObjectAppended(object sender, ObjectEventArgs e)
        {
            ////ObjectId oldlast = Utils.EntNext(e.DBObject.Id);
            //Database db = HostApplicationServices.WorkingDatabase;
            //if (DBObjects.Count == 0)
            //{
            //    DBObjects.Add(e.DBObject);
            //    objectIds.Add(DBObjects[DBObjects.Count - 1].ObjectId);
            //}
            //if (!objectIds.Contains(DBObjects[DBObjects.Count - 1].ObjectId))
            //{
            //    DBObjects.Add(e.DBObject);
            //    objectIds.Add(DBObjects[DBObjects.Count - 1].ObjectId);
            //}
            DBObjects.Add(e.DBObject);
            objectIds.Add(DBObjects[DBObjects.Count - 1].ObjectId);
        }


        [CommandMethod("NHL")]
        public void NHL()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            db.ObjectAppended -= Db_ObjectAppended;

            Entity[] entities = new Entity[DBObjects.Count];
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                for (int i = 0; i < DBObjects.Count; i++)
                {
                    entities[i] = DBObjects[i].Clone() as Entity;
                }
            }
            ReBlockTable.ReBlock(entities.ToArray());
            //using (Transaction trans = db.TransactionManager.StartTransaction())
            //{
            //    for (int i = 0; i < objectIds.Count; i++)
            //    {
            //        //Entity entity = (Entity)objectIds[i].GetObject(OpenMode.ForWrite);
            //        //entity.Erase();
            //        //trans.Commit();
            //    }
            //}
            db.ObjectAppended += Db_ObjectAppended;
            DBObjects.Clear();
            objectIds.Clear();
        }

        [CommandMethod("CloseNHL")]
        public void CloseNHL()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            DBObjects.Clear();
            objectIds.Clear();
            db.ObjectAppended -= Db_ObjectAppended;
        }
        //private void Ed_PromptForEntityEnding(object sender, PromptForEntityEndingEventArgs e)
        //{
        //    ObjectId oldlast = Utils.EntLast();
        //}
    }
}
