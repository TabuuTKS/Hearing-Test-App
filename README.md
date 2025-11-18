<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
   

        
    
</head>
<body class="text-gray-800 leading-relaxed p-4 md:p-8">

  <main class="max-w-4xl mx-auto content-wrapper">
        
       
  <h1 class="text-3xl md:text-4xl font-bold text-gray-900 mb-2">📱 Development of a Mobile-Based System for Human Auditory Acuity Assessment</h1>
        <p class="text-lg text-gray-700 mb-8">
            <strong>A mobile hearing screening tool developed in Unity and C#</strong>
        </p>
        <p class="text-gray-600">
            This repository contains the major project for the Bachelor of Computer Applications program at Jagran Lakecity University. The project is a mobile application designed to conduct a preliminary assessment of a user's hearing acuity, based on the principles of pure-tone audiometry.
        </p>
        
   <hr class="my-8 border-gray-200">

        
   <section>
            <h2 class="text-2xl font-semibold text-gray-900 mb-4">💡 The Problem</h2>
            <p class="mb-4">
                Hearing loss is a significant and growing global health issue, affecting over 1.5 billion people. Traditional clinical hearing exams, while accurate, face significant barriers:
            </p>
            <blockquote>
                <p class="mb-2"><strong>High Cost & Inaccessibility:</strong> Professional exams require expensive, calibrated equipment in sound-proof booths, making them costly and concentrated in specialized clinics.</p>
                <p class="mb-2"><strong>Lack of Routine Screening:</strong> Unlike vision or blood pressure, hearing tests are not a standard part of most adult health check-ups, leading to delayed diagnosis.</p>
                <p><strong>Social Stigma:</strong> Many people are reluctant to seek a formal diagnosis, which prevents early intervention.</p>
            </blockquote>
        </section>

        
   <section class="mt-8">
            <h2 class="text-2xl font-semibold text-gray-900 mb-4">🚀 Our Solution</h2>
            <p class="mb-4">
                This project aims to bridge the gap by providing a <strong>free, accessible, and reliable</strong> mobile screening tool. It allows anyone with a smartphone and a pair of headphones to get a preliminary check of their hearing in a private, convenient setting.
            </p>
            <p class="p-4 bg-yellow-50 border-l-4 border-yellow-400 text-yellow-800 rounded-r-lg">
                The app is <strong>not a medical diagnosis</strong> but serves as a vital first step to empower users, promote early detection, and encourage those with potential hearing loss to seek professional medical advice.
            </p>
        </section>

       
   <section class="mt-8">
            <h2 class="text-2xl font-semibold text-gray-900 mb-6">📊 Key Features</h2>
            <ul class="space-y-6">
                <li>
                    <h3 class="text-xl font-semibold mb-2">1. Comprehensive Patient Questionnaire</h3>
                    <p class="mb-2">Gathers essential user history, including age, gender, noise exposure (occupational, recreational), medical history (diabetes, hypertension), and specific ear-related symptoms (tinnitus, dizziness).</p>
                    <p class="text-sm text-gray-600"><em>Powered by: <code>QuestionnaireManager.cs</code>, <code>PatientData.cs</code></em></p>
                </li>
                <li>
                    <h3 class="text-xl font-semibold mb-2">2. Headphone Calibration</h3>
                    <p class="mb-2">A critical pre-test step that accounts for hardware variance. The user finds their "barely audible" level for each test frequency, creating a personalized baseline (in dBFS) for their specific phone and headphones.</p>
                    <p class="text-sm text-gray-600"><em>Powered by: <code>HearingTestManager.cs</code></em></p>
                </li>
                <li>
                    <h3 class="text-xl font-semibold mb-2">3. Automated Audiometry Test</h3>
                    <p class="mb-2">Performs a pure-tone audiometry test for both left and right ears, testing standard frequencies (250 Hz, 500 Hz, 1000 Hz, 2000 Hz, 4000 Hz, 8000 Hz).</p>
                    <p class="mb-2">The test uses an automated ascending method, presenting tones at increasing volumes until the user responds.</p>
                    <p class="text-sm text-gray-600"><em>Powered by: <code>AudiometryTest.cs</code>, <code>AudioWaves.cs</code></em></p>
                </li>
                <li>
                    <h3 class="text-xl font-semibold mb-2">4. Detailed Results & Audiogram</h3>
                    <p class="mb-2">Generates an immediate, easy-to-understand results screen upon test completion.</p>
                    <p class="mb-2">Displays a visual audiogram chart plotting the hearing thresholds (in dB HL) for each ear.</p>
                    <p class="mb-2">Provides a clear hearing status (e.g., "Normal," "Mild Loss").</p>
                    <p class="mb-2">Analyzes questionnaire and test data to list personalized risk factors and actionable recommendations.</p>
                    <p class="text-sm text-gray-600"><em>Powered by: <code>ResultsManager.cs</code></em></p>
                </li>
                <li>
                    <h3 class="text-xl font-semibold mb-2">5. Exportable Report</h3>
                    <p class="mb-2">Allows the user to download their complete report as a <code>.txt</code> file for their personal records.</p>
                    <p class="text-sm text-gray-600"><em>Powered by: <code>ResultsManager.cs</code></em></p>
                </li>
            </ul>
        </section>

        
   <section class="mt-8">
            <h2 class="text-2xl font-semibold text-gray-900 mb-4">🔧 Tech Stack</h2>
            <div class="overflow-x-auto rounded-lg border border-gray-200">
                <table class="min-w-full text-sm">
                    <thead>
                        <tr>
                            <th>Component</th>
                            <th>Technology</th>
                        </tr>
                    </thead>
                    <tbody class="divide-y divide-gray-200">
                        <tr>
                            <td class="font-medium"><strong>Development Platform</strong></td>
                            <td>Unity 2021.3+</td>
                        </tr>
                        <tr>
                            <td class="font-medium"><strong>Programming Language</strong></td>
                            <td>C#</td>
                        </tr>
                        <tr>
                            <td class="font-medium"><strong>UI System</strong></td>
                            <td>Unity UI (with TextMeshPro)</td>
                        </tr>
                        <tr>
                            <td class="font-medium"><strong>Audio Generation</strong></td>
                            <td>Programmatic Sine Waves (<code>OnAudioFilterRead</code>)</td>
                        </tr>
                        <tr>
                            <td class="font-medium"><strong>Target Platform</strong></td>
                            <td>Android (can also be built for PC)</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </section>

        
   <section class="mt-8">
            <h2 class="text-2xl font-semibold text-gray-900 mb-4">🖥️ Application Workflow & Screenshots</h2>
           
            
   <ol class="list-decimal list-outside space-y-6 ml-5">
                <li>
                    <strong>Main Menu</strong><br>
                    <em class="text-gray-600">The user starts the test.</em>
                    <div class="my-2 p-6 border-2 border-dashed rounded-lg bg-gray-50 text-center text-gray-500">
                        </code>
                    </div>
                </li>
                <li>
                    <strong>Questionnaire</strong><br>
                    <em class="text-gray-600">The user fills out their medical and lifestyle history.</em>
                    <div class="my-2 p-6 border-2 border-dashed rounded-lg bg-gray-50 text-center text-gray-500">
                      </code>
                    </div>
                </li>
                <li>
                    <strong>Calibration</strong><br>
                    <em class="text-gray-600">The user calibrates their headphones in a quiet room.</em>
                    <div class="my-2 p-6 border-2 border-dashed rounded-lg bg-gray-50 text-center text-gray-500">
                       </code>
                    </div>
                </li>
                <li>
                    <strong>Hearing Test</strong><br>
                    <em class="text-gray-600">The user listens for tones and presses "Heard" or "No Response".</em>
                    <div class="my-2 p-6 border-2 border-dashed rounded-lg bg-gray-50 text-center text-gray-500">
                       </code>
                    </div>
                </li>
                <li>
                    <strong>Results</strong><br>
                    <em class="text-gray-600">The user reviews their personalized audiogram, risks, and recommendations.</em>
                    <div class="my-2 p-6 border-2 border-dashed rounded-lg bg-gray-50 text-center text-gray-500">
                       </code>
                    </div>
                </li>
            </ol>
        </section>

        
   <section class="mt-8">
            <h2 class="text-2xl font-semibold text-gray-900 mb-4">Usage</h2>
            <ol class="list-decimal list-outside space-y-2 ml-5 bg-gray-50 p-6 rounded-lg">
                <li>Clone this repository.</li>
                <li>Open the project in <strong>Unity Hub</strong> (requires Unity 2021.3 or newer).</li>
                <li>Open the main scene (e.g., <code>MainScene.unity</code>).</li>
                <li>
                    If building for Android:
                    <ul class="list-disc list-outside ml-5 mt-2">
                        <li>Go to <strong>File > Build Settings</strong>.</li>
                        <li>Select <strong>Android</strong> and click <strong>Switch Platform</strong>.</li>
                        <li>Connect your Android device (with Developer Mode enabled).</li>
                        <li>Click <strong>Build and Run</strong>.</li>
                    </ul>
                </li>
                <li class="mt-2">
                    If running in the Editor:
                    <ul class="list-disc list-outside ml-5 mt-2">
                        <li>Simply press the <strong>Play</strong> button.</li>
                    </ul>
                </li>
            </ol>
        </section>

        
   <section class="mt-8">
            <h2 class="text-2xl font-semibold text-gray-900 mb-4">📚 Project Documentation</h2>
            <p class="mb-3">For a detailed breakdown of the system architecture, algorithms, and project outcomes, please see our full project report and presentation:</p>
            <ul class="list-disc list-outside space-y-2 ml-5">
                <li><a href="https://www.google.com/search?q=path/to/your/Project_Report.pdf" class="text-blue-600 hover:underline font-medium"><strong>Full Project Report (PDF)</strong></a></li>
                <li><a href="https://www.google.com/search?q=./Development%2520of%2520a%2520Mobile-Based%2520System%2520for%2520Human%2520Auditory%2520Acuity%2520Assessment%2520Major%2520Project.pdf" class="text-blue-600 hover:underline font-medium"><strong>Project Presentation (PDF)</strong></a></li>
            </ul>
        </section>

        
   <section class="mt-8">
            <h2 class="text-2xl font-semibold text-gray-900 mb-4">👥 Authors & Supervision</h2>
            <p class="mb-3">This project was created by:</p>
            <ul class="list-none space-y-1 mb-6">
                <li><strong>Vivek Raikwar</strong> (<a href="https://github.com/FlaminPro" class="text-blue-600 hover:underline">@FlaminPro</a>)</li>
                <li><strong>Maanas Menghani</strong> (<a href="https://github.com/TabuuTKS" class="text-blue-600 hover:underline">@TabuuTKS</a>)</li>
                <li><strong>Shivank Chaturvedi</strong> (<a href="https://github.com/caspin1844-dotcom" class="text-blue-600 hover:underline">@caspin1844-dotcom</a>)</li>
            </ul>
            
 

</main>
    
   <footer class="mt-8 pt-6 border-t border-gray-200 text-center text-gray-500 text-sm">
    </footer>

</body>
</html>
