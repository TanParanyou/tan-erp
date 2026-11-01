//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/project_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_cost_ledger_entry_response.g.dart';

/// ProjectCostLedgerEntryResponse
///
/// Properties:
/// * [id] 
/// * [wbsNodeId] 
/// * [wbsCode] 
/// * [category] 
/// * [entryType] 
/// * [sourceType] 
/// * [sourceId] 
/// * [sourceNumber] 
/// * [description] 
/// * [amount] 
/// * [isReversal] 
/// * [actor] 
/// * [occurredAtUtc] 
@BuiltValue()
abstract class ProjectCostLedgerEntryResponse implements Built<ProjectCostLedgerEntryResponse, ProjectCostLedgerEntryResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'wbsNodeId')
  String? get wbsNodeId;

  @BuiltValueField(wireName: r'wbsCode')
  String? get wbsCode;

  @BuiltValueField(wireName: r'category')
  String? get category;

  @BuiltValueField(wireName: r'entryType')
  String? get entryType;

  @BuiltValueField(wireName: r'sourceType')
  String? get sourceType;

  @BuiltValueField(wireName: r'sourceId')
  String? get sourceId;

  @BuiltValueField(wireName: r'sourceNumber')
  String? get sourceNumber;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'amount')
  double? get amount;

  @BuiltValueField(wireName: r'isReversal')
  bool? get isReversal;

  @BuiltValueField(wireName: r'actor')
  ProjectPersonResponse? get actor;

  @BuiltValueField(wireName: r'occurredAtUtc')
  DateTime? get occurredAtUtc;

  ProjectCostLedgerEntryResponse._();

  factory ProjectCostLedgerEntryResponse([void updates(ProjectCostLedgerEntryResponseBuilder b)]) = _$ProjectCostLedgerEntryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectCostLedgerEntryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectCostLedgerEntryResponse> get serializer => _$ProjectCostLedgerEntryResponseSerializer();
}

class _$ProjectCostLedgerEntryResponseSerializer implements PrimitiveSerializer<ProjectCostLedgerEntryResponse> {
  @override
  final Iterable<Type> types = const [ProjectCostLedgerEntryResponse, _$ProjectCostLedgerEntryResponse];

  @override
  final String wireName = r'ProjectCostLedgerEntryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectCostLedgerEntryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.wbsNodeId != null) {
      yield r'wbsNodeId';
      yield serializers.serialize(
        object.wbsNodeId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.wbsCode != null) {
      yield r'wbsCode';
      yield serializers.serialize(
        object.wbsCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.category != null) {
      yield r'category';
      yield serializers.serialize(
        object.category,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.entryType != null) {
      yield r'entryType';
      yield serializers.serialize(
        object.entryType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sourceType != null) {
      yield r'sourceType';
      yield serializers.serialize(
        object.sourceType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sourceId != null) {
      yield r'sourceId';
      yield serializers.serialize(
        object.sourceId,
        specifiedType: const FullType(String),
      );
    }
    if (object.sourceNumber != null) {
      yield r'sourceNumber';
      yield serializers.serialize(
        object.sourceNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.amount != null) {
      yield r'amount';
      yield serializers.serialize(
        object.amount,
        specifiedType: const FullType(double),
      );
    }
    if (object.isReversal != null) {
      yield r'isReversal';
      yield serializers.serialize(
        object.isReversal,
        specifiedType: const FullType(bool),
      );
    }
    if (object.actor != null) {
      yield r'actor';
      yield serializers.serialize(
        object.actor,
        specifiedType: const FullType(ProjectPersonResponse),
      );
    }
    if (object.occurredAtUtc != null) {
      yield r'occurredAtUtc';
      yield serializers.serialize(
        object.occurredAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectCostLedgerEntryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectCostLedgerEntryResponseBuilder result,
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
        case r'wbsNodeId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.wbsNodeId = valueDes;
          break;
        case r'wbsCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.wbsCode = valueDes;
          break;
        case r'category':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.category = valueDes;
          break;
        case r'entryType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.entryType = valueDes;
          break;
        case r'sourceType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceType = valueDes;
          break;
        case r'sourceId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceId = valueDes;
          break;
        case r'sourceNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceNumber = valueDes;
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.amount = valueDes;
          break;
        case r'isReversal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isReversal = valueDes;
          break;
        case r'actor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectPersonResponse),
          ) as ProjectPersonResponse?;
          if (valueDes == null) continue;
          result.actor.replace(valueDes);
          break;
        case r'occurredAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.occurredAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectCostLedgerEntryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectCostLedgerEntryResponseBuilder();
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


