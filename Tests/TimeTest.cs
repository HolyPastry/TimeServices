using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TimeTest
{

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator SetAndGet()
    {
        SceneManager.LoadScene("TimeTestScene");
        yield return TimeServices.WaitUntilReady();
        TimeServices.SetTimeOfDay(new System.DateTime(2021, 1, 1, 12, 0, 0));
        Assert.AreEqual(new System.DateTime(2021, 1, 1, 12, 0, 0), TimeServices.GetTimeOfDay());
        TimeServices.SetHour(13);
        Assert.AreEqual(new System.DateTime(2021, 1, 1, 13, 0, 0), TimeServices.GetTimeOfDay());
        TimeServices.AddHours(1);
        Assert.AreEqual(new System.DateTime(2021, 1, 1, 14, 0, 0), TimeServices.GetTimeOfDay());

        TimeServices.SetTimeFactor(3600);

        yield return new WaitForSeconds(2f);

        Assert.GreaterOrEqual(TimeServices.GetTimeOfDay(), new System.DateTime(2021, 1, 1, 15, 0, 0));

    }

    [UnityTest]
    public IEnumerator PauseAndResume()
    {
        SceneManager.LoadScene("TimeTestScene");
        yield return TimeServices.WaitUntilReady();
        TimeServices.SetTimeOfDay(new System.DateTime(2021, 1, 1, 12, 0, 0));
        TimeServices.SetTimeFactor(3600);
        TimeServices.PauseTimeOfDay();
        yield return new WaitForSeconds(2f);
        var time = TimeServices.GetTimeOfDay();
        yield return new WaitForSeconds(2f);
        Assert.AreEqual(time, TimeServices.GetTimeOfDay());
        TimeServices.ResumeTimeOfDay();
        yield return new WaitForSeconds(2f);
        Assert.Greater(TimeServices.GetTimeOfDay(), time);

    }

    [UnityTest]
    public IEnumerator WaitForHours()
    {
        SceneManager.LoadScene("TimeTestScene");
        yield return TimeServices.WaitUntilReady();
        TimeServices.SetTimeOfDay(new System.DateTime(2021, 1, 1, 12, 0, 0));
        TimeServices.SetTimeFactor(3600);
        yield return TimeServices.WaitForHours(1);

        Assert.GreaterOrEqual(TimeServices.GetTimeOfDay(), new System.DateTime(2021, 1, 1, 13, 0, 0));
        yield return TimeServices.WaitForHours(1);
        Assert.GreaterOrEqual(TimeServices.GetTimeOfDay(), new System.DateTime(2021, 1, 1, 14, 0, 0));
    }

    [UnityTest]
    public IEnumerator IsTimeBetween()
    {
        SceneManager.LoadScene("TimeTestScene");
        yield return TimeServices.WaitUntilReady();
        TimeServices.SetTimeOfDay(new System.DateTime(2021, 1, 1, 12, 0, 0));
        Assert.IsTrue(TimeServices.IsTimeBetween(11, 0, 13, 0));
        Assert.IsFalse(TimeServices.IsTimeBetween(13, 0, 14, 0));
    }

}
