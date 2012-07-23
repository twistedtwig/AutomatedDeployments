using DeploymentConfiguration.Actions;
using DeploymentTask.Tasks.MsDeployTasks;
using NUnit.Framework;

namespace DeploymentTask.Test
{
    [TestFixture]
    public class DeploymentTaskCollectionTests
    {

        [SetUp]
        public void Init()
        {
            
        }

        [Test]
        public void TestCantAddTwoEquivilentTasksToList()
        {
            DeploymentTaskCollection collection = new DeploymentTaskCollection(false, false);

            IisActionComponentGraph componentGraph1 = new IisActionComponentGraph
                                                          {
                                                              ActionType = ActionType.AppPoolRemoval,
                                                              DestinationComputerName = "192.168.10.98:8173",
                                                              PathToConfigFile = "myfile.config",
                                                              SourceContentPath = @"someSource\files\"
                                                          };

            IisActionComponentGraph componentGraph2= new IisActionComponentGraph
                                                          {
                                                              ActionType = ActionType.AppPoolRemoval,
                                                              DestinationComputerName = "192.168.10.98:8173",
                                                              PathToConfigFile = "myfile.config",
                                                              SourceContentPath = @"someSource\files\"
                                                          };

            MsDeployAppPoolRemovalIisDeploymentTask task1 = new MsDeployAppPoolRemovalIisDeploymentTask(componentGraph1);
            MsDeployAppPoolRemovalIisDeploymentTask task2 = new MsDeployAppPoolRemovalIisDeploymentTask(componentGraph2);

            Assert.AreEqual(0, collection.Count);
            collection.Add(task1);
            Assert.AreEqual(1, collection.Count);
            collection.Add(task2);
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void TestSortingWillArrangeTasksInOrder()
        {
            DeploymentTaskCollection collection = new DeploymentTaskCollection(false, false);

            IisActionComponentGraph componentGraph1 = new IisActionComponentGraph
            {
                ActionType = ActionType.AppPoolCreation,
                DestinationComputerName = "192.168.10.98:8173",
                PathToConfigFile = "myfile.config",
                SourceContentPath = @"someSource\files\"
            };

            IisActionComponentGraph componentGraph2 = new IisActionComponentGraph
            {
                ActionType = ActionType.AppPoolRemoval,
                DestinationComputerName = "192.168.10.98:8173",
                PathToConfigFile = "myfile1.config",
                SourceContentPath = @"someSource\files\"
            };

            IisActionComponentGraph componentGraph3 = new IisActionComponentGraph
            {
                ActionType = ActionType.AppPoolRemoval,
                DestinationComputerName = "192.168.10.98:8173",
                PathToConfigFile = "myfile2.config",
                SourceContentPath = @"someSource\files\"
            };











            FileCopyActionComponentGraph componentGraph4 = new FileCopyActionComponentGraph
            {
                ActionType = ActionType.FileDeployment,
                DestinationComputerName = "192.168.10.98:8173",
                SourceContentPath = @"someSource\files\"
            };


            var task1 = new MsDeployAppPoolInstallIisDeploymentTask(componentGraph1);
            var task2 = new MsDeployAppPoolRemovalIisDeploymentTask(componentGraph2);
            var task3 = new MsDeployAppPoolRemovalIisDeploymentTask(componentGraph3);
            
            
            
            var task4 = new MsDeployFileCopyDeploymentTask(componentGraph4);


            Assert.AreEqual(0, collection.Count);
            collection.Add(task1);
            collection.Add(task2);
            collection.Add(task3);
            collection.Add(task4);

            Assert.AreEqual(task1, collection[0]);
            Assert.AreEqual(task2, collection[1]);
            Assert.AreEqual(task3, collection[2]);
            Assert.AreEqual(task4, collection[3]);

            collection.Sort();

            Assert.AreEqual(task2, collection[0]);
            Assert.AreEqual(task3, collection[1]);
            Assert.AreEqual(task4, collection[2]);
            Assert.AreEqual(task1, collection[3]);


        }

    }
}
