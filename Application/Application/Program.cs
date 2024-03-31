using System;
using Kitware.VTK;

namespace Application
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // Создаем объект DICOM reader
            vtkDICOMImageReader reader = new vtkDICOMImageReader();

            // Устанавливаем путь к папке с DICOM снимками
            reader.SetDirectoryName("D:\\ProgrammingAndProjects\\Studies\\4sem\\OOP_CourseWork\\SE000001");

            // Обновляем reader, чтобы прочитать все снимки
            reader.Update();

            // Получаем объект vtkImageData из reader
            vtkImageData imageData = reader.GetOutput();

            // Создаем объект dual contouring фильтра
            vtkContourFilter contourFilter = new vtkContourFilter();

            // Устанавливаем входные данные
            contourFilter.SetInput(imageData);

            // Устанавливаем значение изоповерхности
            contourFilter.SetValue(0, 150);

            // Обновляем фильтр, тем самым применяя алгоритм dual contouring к срезам
            contourFilter.Update();

            // Создаем объект vtkSTLWriter
            vtkSTLWriter stlWriter = new vtkSTLWriter();

            // Устанавливаем входные данные
            stlWriter.SetInputConnection(contourFilter.GetOutputPort());

            // Устанавливаем имя файла
            stlWriter.SetFileName("D:\\ProgrammingAndProjects\\Studies\\4sem\\OOP_CourseWork\\OOP_Course\\output1.stl");

            // Сохраняем результат
            stlWriter.Write();

            Console.WriteLine("Файл сохранён!");

            // Создаем объект vtkDataSetMapper, который используется для отображения полигональных данных
            // В результате мы получаем укзаатель на класс vtkPolyDataMapper
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();

            // Устанавливаем входные данные
            mapper.SetInputConnection(contourFilter.GetOutputPort());

            // Создаем объект vtkActor, который представялет собой сущность на сцене
            vtkActor actor = new vtkActor();

            // Устанавливаем mapper для actor
            actor.SetMapper(mapper);

            // Создаем объект vtkRenderer, который используется для отображения сцены
            vtkRenderer renderer = vtkRenderer.New();

            // Добавляем actor в renderer
            renderer.AddActor(actor);

            // Устанавливаем цвет фона
            renderer.SetBackground(0, 0, 0);

            // Создаем объект vtkRenderWindow, который используется для отображения сцены в окне
            vtkRenderWindow renderWindow = vtkRenderWindow.New();

            // Устанавливаем renderer для renderWindow
            renderWindow.AddRenderer(renderer);

            // Устанавливаем размер окна
            renderWindow.SetSize(640, 480);

            // Создаем объект vtkRenderWindowInteractor, который используется для взаимодействия с окном 
            vtkRenderWindowInteractor renderWindowInteractor = new vtkRenderWindowInteractor();

            // Устанавливаем renderWindow для renderWindowInteractor
            renderWindowInteractor.SetRenderWindow(renderWindow);

            // Вызываем метод, который инициализирует интерактивное окно визуализации
            renderWindowInteractor.Initialize();
            // Вызываем метод, который выполняет рендеринг сцены в окне визуализации
            renderWindow.Render();
            // Вызываем метод, который запускает интерактивное окно визуализации
            renderWindowInteractor.Start();
        }
    }
}
