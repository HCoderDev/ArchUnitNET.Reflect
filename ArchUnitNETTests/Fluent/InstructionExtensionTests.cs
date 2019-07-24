/*
 * Copyright 2019 TNG Technology Consulting GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArchUnitNET.Fluent;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xunit;

namespace ArchUnitNETTests.Fluent
{
    public class InstructionExtensionTests
    {
        private readonly Instruction _nullInstruction = null;
        private readonly Instruction _nextOpNull;
        private readonly Instruction _nextOperandIsNotFieldReference;

        public InstructionExtensionTests()
        {
            var mockModuleDefinition = LoadMockModule(typeof(InstructionExtensionTests).Assembly.Location);
            _nextOpNull = mockModuleDefinition.Assembly.Modules.First().Types
                .First(type => type.Name == "ClassWithPropertyA").Methods.First().Body.Instructions
                .First(instruction => instruction.OpCode.Equals(OpCodes.Ret));
            _nextOperandIsNotFieldReference = mockModuleDefinition.Assembly.Modules.First().Types
                .First(type => type.Name == "ClassWithMethodSignatureA").Methods.First().Body.Instructions
                .First(instruction => instruction.OpCode.Equals(OpCodes.Newobj));
        }

        [Theory]
        [ClassData(typeof(InstructionExtensionBuild.InstructionExtensionTestData))]
        public void InstructionExtensionMethodsProperlyHandleNullArgument(Func<Instruction, bool> instructionFunc)
        {
            Assert.Throws<ArgumentNullException>(() => instructionFunc(_nullInstruction));
        }

        [Fact]
        public void GetAssigneeFieldDefinitionProperlyHandlesNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => _nullInstruction.GetAssigneeFieldDefinition());
        }

        [Fact]
        public void GetAssigneeFieldDefinitionProperlyHandlesNextOpNull()
        {
            Assert.Null(_nextOpNull.GetAssigneeFieldDefinition());
        }

        [Fact]
        public void GetAssigneeFieldDefinitionProperlyHandlesNextOpNotFieldReference()
        {
            Assert.Null(_nextOperandIsNotFieldReference.GetAssigneeFieldDefinition());
        }

        [Fact]
        public void EndOfOperationsDetectedForSetBackingField()
        {
            Assert.False(_nextOpNull.IsMethodCallToSetBackingFieldDefinition());
        }

        private static ModuleDefinition LoadMockModule(string fileName)
        {
            try
            {
                return ModuleDefinition.ReadModule(fileName, new ReaderParameters());
            }
            catch (BadImageFormatException)
            {
                // invalid file format of DLL or executable, therefore ignored
            }

            return null;
        }
        
    }

    public class InstructionExtensionBuild
    {
        public class InstructionExtensionTestData : IEnumerable<object[]>
        {
            private readonly List<object[]> _instructionExtensionData = new List<object[]>
            {
                BuildInstructionExtensionTestData(InstructionExtensions.IsOperationForBackedProperty),
                BuildInstructionExtensionTestData(InstructionExtensions.IsMethodCallAssignment),
                BuildInstructionExtensionTestData(InstructionExtensions.IsMethodCallOp),
                BuildInstructionExtensionTestData(InstructionExtensions.IsMethodCallToSetBackingFieldDefinition),
                BuildInstructionExtensionTestData(InstructionExtensions.IsReturnOp),
                BuildInstructionExtensionTestData(InstructionExtensions.IsNewObjectOp),
                BuildInstructionExtensionTestData(InstructionExtensions.IsOperandBackingField),
                BuildInstructionExtensionTestData(InstructionExtensions.IsSetFieldOp),
                BuildInstructionExtensionTestData(InstructionExtensions.IsNewObjectToSetBackingField)
                
            };

            public IEnumerator<object[]> GetEnumerator()
            {
                return _instructionExtensionData.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private static object[] BuildInstructionExtensionTestData(Func<Instruction, bool> instructionFunc)
        {
            return new object[] {instructionFunc};
        }
    }
}