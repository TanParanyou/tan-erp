//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/project_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_change_order_response.g.dart';

/// ProjectChangeOrderResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [title] 
/// * [reason] 
/// * [budgetDelta] 
/// * [contractDelta] 
/// * [status] 
/// * [createdBy] 
/// * [createdAtUtc] 
/// * [submittedAtUtc] 
/// * [decidedBy] 
/// * [decidedAtUtc] 
/// * [decisionNote] 
/// * [rowVersion] 
@BuiltValue()
abstract class ProjectChangeOrderResponse implements Built<ProjectChangeOrderResponse, ProjectChangeOrderResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'title')
  String? get title;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'budgetDelta')
  double? get budgetDelta;

  @BuiltValueField(wireName: r'contractDelta')
  double? get contractDelta;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'createdBy')
  ProjectPersonResponse? get createdBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'submittedAtUtc')
  DateTime? get submittedAtUtc;

  @BuiltValueField(wireName: r'decidedBy')
  ProjectPersonResponse? get decidedBy;

  @BuiltValueField(wireName: r'decidedAtUtc')
  DateTime? get decidedAtUtc;

  @BuiltValueField(wireName: r'decisionNote')
  String? get decisionNote;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  ProjectChangeOrderResponse._();

  factory ProjectChangeOrderResponse([void updates(ProjectChangeOrderResponseBuilder b)]) = _$ProjectChangeOrderResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectChangeOrderResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectChangeOrderResponse> get serializer => _$ProjectChangeOrderResponseSerializer();
}

class _$ProjectChangeOrderResponseSerializer implements PrimitiveSerializer<ProjectChangeOrderResponse> {
  @override
  final Iterable<Type> types = const [ProjectChangeOrderResponse, _$ProjectChangeOrderResponse];

  @override
  final String wireName = r'ProjectChangeOrderResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectChangeOrderResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.title != null) {
      yield r'title';
      yield serializers.serialize(
        object.title,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.budgetDelta != null) {
      yield r'budgetDelta';
      yield serializers.serialize(
        object.budgetDelta,
        specifiedType: const FullType(double),
      );
    }
    if (object.contractDelta != null) {
      yield r'contractDelta';
      yield serializers.serialize(
        object.contractDelta,
        specifiedType: const FullType(double),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(ProjectPersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.submittedAtUtc != null) {
      yield r'submittedAtUtc';
      yield serializers.serialize(
        object.submittedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.decidedBy != null) {
      yield r'decidedBy';
      yield serializers.serialize(
        object.decidedBy,
        specifiedType: const FullType(ProjectPersonResponse),
      );
    }
    if (object.decidedAtUtc != null) {
      yield r'decidedAtUtc';
      yield serializers.serialize(
        object.decidedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.decisionNote != null) {
      yield r'decisionNote';
      yield serializers.serialize(
        object.decisionNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectChangeOrderResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectChangeOrderResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'title':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.title = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'budgetDelta':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.budgetDelta = valueDes;
          break;
        case r'contractDelta':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.contractDelta = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectPersonResponse),
          ) as ProjectPersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'submittedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.submittedAtUtc = valueDes;
          break;
        case r'decidedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectPersonResponse),
          ) as ProjectPersonResponse?;
          if (valueDes == null) continue;
          result.decidedBy.replace(valueDes);
          break;
        case r'decidedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.decidedAtUtc = valueDes;
          break;
        case r'decisionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.decisionNote = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectChangeOrderResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectChangeOrderResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


