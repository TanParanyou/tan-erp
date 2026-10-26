//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'commit_item_import_request.g.dart';

/// CommitItemImportRequest
///
/// Properties:
/// * [content] 
/// * [expectedContentSha256] 
/// * [kind] 
/// * [mode] 
@BuiltValue()
abstract class CommitItemImportRequest implements Built<CommitItemImportRequest, CommitItemImportRequestBuilder> {
  @BuiltValueField(wireName: r'content')
  String? get content;

  @BuiltValueField(wireName: r'expectedContentSha256')
  String? get expectedContentSha256;

  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'mode')
  String? get mode;

  CommitItemImportRequest._();

  factory CommitItemImportRequest([void updates(CommitItemImportRequestBuilder b)]) = _$CommitItemImportRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CommitItemImportRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CommitItemImportRequest> get serializer => _$CommitItemImportRequestSerializer();
}

class _$CommitItemImportRequestSerializer implements PrimitiveSerializer<CommitItemImportRequest> {
  @override
  final Iterable<Type> types = const [CommitItemImportRequest, _$CommitItemImportRequest];

  @override
  final String wireName = r'CommitItemImportRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CommitItemImportRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.content != null) {
      yield r'content';
      yield serializers.serialize(
        object.content,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.expectedContentSha256 != null) {
      yield r'expectedContentSha256';
      yield serializers.serialize(
        object.expectedContentSha256,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.mode != null) {
      yield r'mode';
      yield serializers.serialize(
        object.mode,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CommitItemImportRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CommitItemImportRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'content':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.content = valueDes;
          break;
        case r'expectedContentSha256':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedContentSha256 = valueDes;
          break;
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'mode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.mode = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CommitItemImportRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CommitItemImportRequestBuilder();
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


